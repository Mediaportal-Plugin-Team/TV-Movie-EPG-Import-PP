#Region "Copyright (C) 2005-2025 Team MediaPortal"

' Copyright (C) 2005-2025 Team MediaPortal
' http://www.team-mediaportal.com
' 
' MediaPortal is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 2 of the License, or
' (at your option) any later version.
' 
' MediaPortal is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#End Region

imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.OleDb
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Threading
Imports TvDatabase
Imports TvLibrary.Interfaces
Imports TvLibrary.Log





Namespace TvEngine

	#Region "TVMChannel struct"

	Public Structure TVMChannel
		Private fID As String
		Private fSenderKennung As String
		Private fBezeichnung As String
		Private fWebseite As String
		Private fSortNrTVMovie As String
		Private fZeichen As String

		Public Sub New(aID As String, aSenderKennung As String, aBezeichnung As String, aWebseite As String, aSortNrTVMovie As String, aZeichen As String)
			fID = aID
			fSenderKennung = aSenderKennung
			fBezeichnung = aBezeichnung
			fWebseite = aWebseite
			fSortNrTVMovie = aSortNrTVMovie
			fZeichen = aZeichen
		End Sub

		Public ReadOnly Property TvmId() As String
			Get
				Return fID
			End Get
		End Property

		Public ReadOnly Property TvmEpgChannel() As String
			Get
				Return fSenderKennung
			End Get
		End Property

		Public ReadOnly Property TvmEpgDescription() As String
			Get
				Return fBezeichnung
			End Get
		End Property

		Public ReadOnly Property TvmWebLink() As String
			Get
				Return fWebseite
			End Get
		End Property

		Public ReadOnly Property TvmSortId() As String
			Get
				Return fSortNrTVMovie
			End Get
		End Property

		Public ReadOnly Property TvmZeichen() As String
			Get
				Return fZeichen
			End Get
		End Property
	End Structure

	#End Region

	Friend Class TvMovieDatabase
		#Region "Members"

		Private _databaseConnection As OleDbConnection = Nothing
		Private _canceled As Boolean = False
		Private _tvmEpgChannels As List(Of TVMChannel)
		Private _tvmEpgProgs As New List(Of Program)(500)
		Private _channelList As List(Of Channel) = Nothing
		Private _programsCounter As Integer = 0
		Private _useShortProgramDesc As Boolean = False
		Private _extendDescription As Boolean = True
		Private _showRatings As Boolean = True
		Private _showAudioFormat As Boolean = False
		Private _slowImport As Boolean = True
		Private _actorCount As Integer = 5
		Private _showLive As Boolean = True
		Private _showRepeat As Boolean = False

		Private Shared _xmlFile As String
		Private Shared _tvbLayer As TvBusinessLayer

		#End Region

		#Region "Events"

		Public Delegate Sub ProgramsChanged(value As Integer, maximum As Integer, text As String)

		'public event ProgramsChanged OnProgramsChanged;
		Public Delegate Sub StationsChanged(value As Integer, maximum As Integer, text As String)

		Public Event OnStationsChanged As StationsChanged

		#End Region

		#Region "Mapping struct"

		Private Structure Mapping
			Private _mpChannel As String
			Private _tvmEpgChannel As String
			Private _mpIdChannel As Integer
			Private _start As system.timespan
			Private _end As system.timespan

			Public Sub New(mpChannel As String, mpIdChannel As Integer, tvmChannel As String, start As String, [end] As String)
				_mpChannel = mpChannel
				_mpIdChannel = mpIdChannel
				_tvmEpgChannel = tvmChannel
				_start = CleanInput(start)
				_end = CleanInput([end])
			End Sub

			#Region "struct properties"

			Public ReadOnly Property Channel() As String
				Get
					Return _mpChannel
				End Get
			End Property

			Public ReadOnly Property IdChannel() As Integer
				Get
					Return _mpIdChannel
				End Get
			End Property

			Public ReadOnly Property TvmEpgChannel() As String
				Get
					Return _tvmEpgChannel
				End Get
			End Property

			Public ReadOnly Property Start() As system.timespan
				Get
					Return _start
				End Get
			End Property

			Public ReadOnly Property [End]() As system.timespan
				Get
					Return _end
				End Get
			End Property

			Private Shared Function CleanInput(input As String) As system.timespan
				Dim hours As Integer = 0
				Dim minutes As Integer = 0
				input = input.Trim()
				Dim index As Integer = input.IndexOf(":"C)
				If index > 0 Then
					hours = Convert.ToInt16(input.Substring(0, index))
				End If
				If index + 1 < input.Length Then
					minutes = Convert.ToInt16(input.Substring(index + 1))
				End If

				If hours > 23 Then
					hours = 0
				End If

				If minutes > 59 Then
					minutes = 0
				End If

				Return New system.timespan(hours, minutes, 0)
			End Function

			#End Region
		End Structure

		#End Region

		#Region "Properties"

		Public ReadOnly Property Stations() As List(Of TVMChannel)
			Get
				Return _tvmEpgChannels
			End Get
		End Property

		Public Property Canceled() As Boolean
			Get
				Return _canceled
			End Get
			Set
				_canceled = value
			End Set
		End Property

		Public ReadOnly Property Programs() As Integer
			Get
				Return _programsCounter
			End Get
		End Property

		Public Shared ReadOnly Property TvBLayer() As TvBusinessLayer
			Get
				If _tvbLayer Is Nothing Then
					_tvbLayer = New TvBusinessLayer()
				End If
				Return _tvbLayer
			End Get
		End Property

		#End Region

		#Region "Public functions"

		Public Function GetChannels() As List(Of Channel)
			Dim tvChannels As New List(Of Channel)()
			Try
				Dim allChannels As IList(Of Channel) = Channel.ListAll()
				For Each channel__1 As Channel In allChannels
					If channel__1.IsTv AndAlso channel__1.VisibleInGuide Then
						tvChannels.Add(channel__1)
					End If
				Next
			Catch ex As Exception
				Log.Info("TVMovie: Exception in GetChannels: {0}" & vbLf & "{1}", ex.Message, ex.StackTrace)
			End Try
			Return tvChannels
		End Function

		Public Function Connect() As Boolean
			LoadMemberSettings()

			Dim dataProviderString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Mode=Share Deny None;Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;"
			If TvMovie.DatabasePath <> [String].Empty Then
				dataProviderString = String.Format(dataProviderString, TvMovie.DatabasePath)
			Else
				Return False
			End If

			Try
				_databaseConnection = New OleDbConnection(dataProviderString)
			Catch connex As Exception
				Log.Info("TVMovie: Exception creating OleDbConnection: {0}", connex.Message)
				Return False
			End Try

			Dim sqlSelect As String = "SELECT * FROM Sender WHERE (Favorit = true) AND (GueltigBis >=Now()) ORDER BY Bezeichnung ASC;"

			Dim tvMovieTable As New DataSet("Sender")
			Try
				_databaseConnection.Open()
				Using databaseCommand As New OleDbCommand(sqlSelect, _databaseConnection)
					Using databaseAdapter As New OleDbDataAdapter(databaseCommand)
						Try
							databaseAdapter.FillSchema(tvMovieTable, SchemaType.Source, "Sender")
							databaseAdapter.Fill(tvMovieTable)
						Catch dsex As Exception
							Log.Info("TVMovie: Exception filling Sender DataSet - {0}" & vbLf & "{1}", dsex.Message, dsex.StackTrace)
							Return False
						End Try
					End Using
				End Using
			Catch ex As System.Data.OleDb.OleDbException
				Log.Info("TVMovie: Error accessing TV Movie Clickfinder database while reading stations: {0}", ex.Message)
				Log.Info("TVMovie: Exception: {0}", ex.StackTrace)
				_canceled = True
				Return False
			Catch ex2 As Exception
				Log.Info("TVMovie: Exception: {0}, {1}", ex2.Message, ex2.StackTrace)
				_canceled = True
				Return False
			Finally
				_databaseConnection.Close()
			End Try

			Try
				_tvmEpgChannels = New List(Of TVMChannel)()
				For Each sender As DataRow In tvMovieTable.Tables("Table").Rows
					Dim senderId As String = sender("ID").ToString()
					Dim senderKennung As String = sender("SenderKennung").ToString()
					Dim senderBez As String = sender("Bezeichnung").ToString()
					' these are non-vital for now.
					Dim senderUrl As String = [String].Empty
					Dim senderSort As String = "-1"
					Dim senderZeichen As String = "tvmovie_senderlogoplatzhalter.gif"
					' Somehow TV Movie's db does not necessarily contain these columns...
					Try
						senderUrl = sender("Webseite").ToString()
					Catch generatedExceptionName As Exception
					End Try
					Try
						senderSort = sender("SortNrTVMovie").ToString()
					Catch generatedExceptionName As Exception
					End Try
					Try
						senderZeichen = sender("Zeichen").ToString()
					Catch generatedExceptionName As Exception
					End Try

					Dim current As New TVMChannel(senderId, senderKennung, senderBez, senderUrl, senderSort, senderZeichen)
					_tvmEpgChannels.Add(current)
				Next
			Catch ex As Exception
				Log.Info("TVMovie: Exception: {0}, {1}", ex.Message, ex.StackTrace)
			End Try

			_channelList = GetChannels()
			Return True
		End Function

		Public ReadOnly Property NeedsImport() As Boolean
			Get
				Try
					Dim restTime As New system.timespan(Convert.ToInt32(TvBLayer.GetSetting("TvMovieRestPeriod", "24").Value), 0, 0)
					Dim lastUpdated As DateTime = Convert.ToDateTime(TvBLayer.GetSetting("TvMovieLastUpdate", "0").Value)
					'        if (Convert.ToInt64(TvBLayer.GetSetting("TvMovieLastUpdate", "0").Value) == LastUpdate)
					If lastUpdated >= (DateTime.Now - restTime) Then
						Return False
					Else
						Log.Debug("TVMovie: Last update was at {0} - new import scheduled", Convert.ToString(lastUpdated))
						Return True
					End If
				Catch ex As Exception
					Log.Info("TVMovie: An error occured checking the last import time {0}", ex.Message)
					Log.Write(ex)
					Return True
				End Try
			End Get
		End Property

		''' <summary>
		''' Loops through all channel to find mappings and finally import EPG to MP's DB
		''' </summary>
		Public Sub Import()
			If _canceled Then
				Return
			End If

			Dim mappingList As List(Of Mapping) = GetMappingList()
			If mappingList Is Nothing OrElse mappingList.Count < 1 Then
				Log.Info("TVMovie: Cannot import from TV Movie database - no mappings found")
				Return
			End If

			Dim ImportStartTime As DateTime = DateTime.Now
			Log.Debug("TVMovie: Importing database")
			Dim maximum As Integer = 0

			For Each tvmChan As TVMChannel In _tvmEpgChannels
				For Each mapping As Mapping In mappingList
					If mapping.TvmEpgChannel = tvmChan.TvmEpgChannel Then
						maximum += 1
						Exit For
					End If
				Next
			Next

			Log.Debug("TVMovie: Calculating stations done")

			' setting update time of epg import to avoid that the background thread triggers another import
			' if the process lasts longer than the timer's update check interval
			Dim setting As Setting = TvBLayer.GetSetting("TvMovieLastUpdate")
			setting.Value = DateTime.Now.ToString()
			setting.Persist()

			Log.Debug("TVMovie: Mapped {0} stations for EPG import", Convert.ToString(maximum))
			Dim counter As Integer = 0

			_tvmEpgProgs.Clear()

			' get all tv channels from MP DB via gentle.net
			Dim allChannels As IList(Of Channel) = Channel.ListAll()

			For Each station As TVMChannel In _tvmEpgChannels
				If _canceled Then
					Return
				End If

				Log.Info("TVMovie: Searching time share mappings for station: {0}", station.TvmEpgDescription)
				' get all tv movie channels
				Dim channelNames As New List(Of Mapping)()

				For Each mapping As Mapping In mappingList
					If mapping.TvmEpgChannel = station.TvmEpgChannel Then
						channelNames.Add(mapping)
					End If
				Next

				If channelNames.Count > 0 Then
					Try
						Dim display As String = [String].Empty
						For Each channelName As Mapping In channelNames
							display += String.Format("{0}  /  ", channelName.Channel)
						Next

						display = display.Substring(0, display.Length - 5)
						RaiseEvent OnStationsChanged(counter, maximum, display)
						counter += 1

						Log.Info("TVMovie: Importing {3} time frame(s) for MP channel [{0}/{1}] - {2}", Convert.ToString(counter), Convert.ToString(maximum), display, Convert.ToString(channelNames.Count))

						_tvmEpgProgs.Clear()

						_programsCounter += ImportStation(station.TvmEpgChannel, channelNames, allChannels)

						Dim importPrio As ThreadPriority = If(_slowImport, ThreadPriority.BelowNormal, ThreadPriority.AboveNormal)
						If _slowImport Then
							Thread.Sleep(32)
						End If

						' make a copy of this list because Insert it done in syncronized threads - therefore the object reference would cause multiple/missing entries
						Dim InsertCopy As New List(Of Program)(_tvmEpgProgs)
						Dim debugCount As Integer = TvBLayer.InsertPrograms(InsertCopy, DeleteBeforeImportOption.OverlappingPrograms, importPrio)
                        Log.Info("TVMovie: Inserted {0} programs", debugCount)

					Catch ex As Exception
						Log.Info("TVMovie: Error inserting programs - {0}", ex.StackTrace)
					End Try
				End If
			Next

			Log.Debug("TVMovie: Waiting for database to be updated...")
			TvBLayer.WaitForInsertPrograms()
			Log.Debug("TVMovie: Database update finished.")


			RaiseEvent OnStationsChanged(maximum, maximum, "Import done")

			If Not _canceled Then
				Try
					setting = TvBLayer.GetSetting("TvMovieLastUpdate")
					setting.Value = DateTime.Now.ToString()
					setting.Persist()

					Dim ImportDuration As system.timespan = (DateTime.Now - ImportStartTime)
                    Log.Debug("TVMovie: Imported {0} database entries for {1} stations in {2} seconds", _programsCounter, counter, Convert.ToString(ImportDuration.TotalSeconds))

                    MsgBox("hallo")

                    If _tvbLayer.GetSetting("TvMovieRunAppAfter").Value IsNot String.Empty Then
                        If File.Exists(_tvbLayer.GetSetting("TvMovieRunAppAfter").Value) Then
                            Process.Start(_tvbLayer.GetSetting("TvMovieRunAppAfter").Value)
                        Else
                            Log.Error("TVMovie: Error - RunApplicationAfter not found")
                        End If
                    End If


                Catch generatedExceptionName As Exception
                    Log.Info("TVMovie: Error updating the database with last import date")
                End Try
			End If
			GC.Collect()
		End Sub

		#End Region

		#Region "Private functions"

		Private Sub LoadMemberSettings()
			_useShortProgramDesc = TvBLayer.GetSetting("TvMovieShortProgramDesc", "false").Value = "true"
			_extendDescription = TvBLayer.GetSetting("TvMovieExtendDescription", "true").Value = "true"
			_showRatings = TvBLayer.GetSetting("TvMovieShowRatings", "true").Value = "true"
			_showAudioFormat = TvBLayer.GetSetting("TvMovieShowAudioFormat", "false").Value = "true"
			_slowImport = TvBLayer.GetSetting("TvMovieSlowImport", "true").Value = "true"
			_actorCount = Convert.ToInt32(TvBLayer.GetSetting("TvMovieLimitActors", "5").Value)
			_showLive = TvBLayer.GetSetting("TvMovieShowLive", "true").Value = "true"
			_showRepeat = TvBLayer.GetSetting("TvMovieShowRepeating", "false").Value = "true"
			_xmlFile = [String].Format("{0}\TVMovieMapping.xml", PathManager.GetDataPath)
		End Sub

		Private Function ImportStation(stationName As String, channelNames As List(Of Mapping), allChannels As IList(Of Channel)) As Integer
			Dim counter As Integer = 0
			Dim sqlSelect As String = [String].Empty
			Dim sqlb As New StringBuilder()

			' UNUSED: F16zu9 , live , untertitel , Dauer , Wiederholung
			'sqlb.Append("SELECT * "); // need for saver schema filling
			sqlb.Append("SELECT TVDaten.SenderKennung, TVDaten.Beginn, TVDaten.Ende, TVDaten.Sendung, TVDaten.Genre, TVDaten.Kurzkritik, TVDaten.KurzBeschreibung, TVDaten.Beschreibung")
			sqlb.Append(", TVDaten.Audiodescription, TVDaten.DolbySuround, TVDaten.Stereo, TVDaten.DolbyDigital, TVDaten.Dolby, TVDaten.Zweikanalton")
			sqlb.Append(", TVDaten.FSK, TVDaten.Herstellungsjahr, TVDaten.Originaltitel, TVDaten.Regie, TVDaten.Darsteller")
			sqlb.Append(", TVDaten.Interessant, TVDaten.Bewertungen")
			sqlb.Append(", TVDaten.live, TVDaten.Dauer, TVDaten.Herstellungsland,TVDaten.Wiederholung")
			sqlb.Append(" FROM TVDaten WHERE (((TVDaten.SenderKennung)=""{0}"") AND ((TVDaten.Ende)>= #{1}#)) ORDER BY TVDaten.Beginn;")

			Dim importTime As DateTime = DateTime.Now.Subtract(system.timespan.FromHours(4))
			sqlSelect = String.Format(sqlb.ToString(), stationName, importTime.ToString("yyyy-MM-dd HH:mm:ss"))
			'("dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture));
			Dim databaseTransaction As OleDbTransaction = Nothing
			Using databaseCommand As New OleDbCommand(sqlSelect, _databaseConnection)
				Try
					_databaseConnection.Open()
					' The main app might change epg details while importing
					databaseTransaction = _databaseConnection.BeginTransaction(IsolationLevel.ReadCommitted)
					databaseCommand.Transaction = databaseTransaction
					Using reader As OleDbDataReader = databaseCommand.ExecuteReader(CommandBehavior.SequentialAccess)
						While reader.Read()
							ImportSingleChannelData(channelNames, allChannels, counter, reader(0).ToString(), reader(1).ToString(), reader(2).ToString(), _
								reader(3).ToString(), reader(4).ToString(), reader(5).ToString(), reader(6).ToString(), reader(7).ToString(), reader(8).ToString(), _
								reader(9).ToString(), reader(10).ToString(), reader(11).ToString(), reader(12).ToString(), reader(13).ToString(), reader(14).ToString(), _
								reader(15).ToString(), reader(16).ToString(), reader(17).ToString(), reader(18).ToString(), reader(19).ToString(), reader(20).ToString(), _
								reader(21).ToString(), reader(22).ToString(), reader(23).ToString(), reader(24).ToString())
							counter += 1
						End While
						databaseTransaction.Commit()
						reader.Close()
					End Using
				Catch ex As OleDbException
					databaseTransaction.Rollback()
					Log.Info("TVMovie: Error accessing TV Movie Clickfinder database - import of current station canceled")
					Log.[Error]("TVMovie: Exception: {0}", ex)
					Return 0
				Catch ex1 As Exception
					Try
						databaseTransaction.Rollback()
					Catch generatedExceptionName As Exception
					End Try
					Log.Info("TVMovie: Exception: {0}", ex1)
					Return 0
				Finally
					_databaseConnection.Close()
				End Try
			End Using

			Return counter
		End Function

		' sqlb.Append(", TVDaten.live, TVDaten.Dauer, TVDaten.Herstellungsland,TVDaten.Wiederholung");
		''' <summary>
		''' Takes a DataRow worth of EPG Details to persist them in MP's program table
		''' </summary>
		Private Sub ImportSingleChannelData(channelNames As List(Of Mapping), allChannels As IList(Of Channel), aCounter As Integer, SenderKennung As String, Beginn As String, Ende As String, _
			Sendung As String, Genre__1 As String, Kurzkritik As String, KurzBeschreibung As String, Beschreibung As String, Audiodescription As String, _
			DolbySuround As String, Stereo As String, DolbyDigital As String, Dolby As String, Zweikanalton As String, FSK As String, _
			Herstellungsjahr As String, Originaltitel As String, Regie As String, Darsteller As String, Interessant As String, Bewertungen As String, _
			Live__2 As String, Dauer As String, Herstellungsland As String, Wiederholung As String)
			Dim channel As String = SenderKennung
			Dim [end] As DateTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value
			Dim start As DateTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value
			Dim classification As String = [String].Empty
			Dim parentalRating As Integer = 0
			Dim [date] As String = [String].Empty
			Dim duration As String = [String].Empty
			Dim origin As String = [String].Empty
			Dim episode As String = [String].Empty
			Dim starRating As Integer = -1
			Dim detailedRating As String = [String].Empty
			Dim director As String = [String].Empty
			Dim actors As String = [String].Empty
			Dim audioFormat As String = [String].Empty
			Dim live__3 As Boolean = False
			Dim repeating As Boolean = False
			Try
				[end] = DateTime.Parse(Ende)
				' iEndTime ==> Ende  (15.06.2006 22:45:00 ==> 20060615224500)
				start = DateTime.Parse(Beginn)
				' iStartTime ==> Beginn (15.06.2006 22:45:00 ==> 20060615224500)
				live__3 = Convert.ToBoolean(Live__2)
				repeating = Convert.ToBoolean(Wiederholung)
			Catch ex2 As Exception
				Log.Info("TVMovie: Error parsing EPG time data - {0}", ex2.ToString())
			End Try

			Dim title As String = Sendung
			' indicate live programs
			If _showLive Then
				If live__3 Then
					title += " (LIVE)"
				End If
			End If
			' indicate repeatings
			If _showRepeat Then
				If repeating Then
					title += " (Wdh.)"
				End If
			End If

			Dim genre__4 As String = Genre__1
			Dim shortCritic As String = Kurzkritik

			If _extendDescription Then
				classification = FSK
				Integer.TryParse(FSK, parentalRating)
				[date] = Herstellungsjahr
				episode = Originaltitel
				director = Regie
				actors = Darsteller
				duration = Dauer
					'int repeat = Convert.ToInt16(guideEntry["Wiederholung"]);         // strRepeat ==> Wiederholung "Repeat" / "unknown"      
				origin = Herstellungsland
			End If

			If _showRatings Then
				starRating = Convert.ToInt16(Interessant) - 1
				detailedRating = Bewertungen
			End If

			Dim EPGStarRating As Short = -1
			Select Case starRating
				Case 0
					EPGStarRating = 2
					Exit Select
				Case 1
					EPGStarRating = 4
					Exit Select
				Case 2
					EPGStarRating = 6
					Exit Select
				Case 3
					EPGStarRating = 8
					Exit Select
				Case 4
					EPGStarRating = 10
					Exit Select
				Case 5
					EPGStarRating = 8
					Exit Select
				Case 6
					EPGStarRating = 10
					Exit Select
				Case Else
					EPGStarRating = -1
					Exit Select
			End Select

			For Each channelMap As Mapping In channelNames
				Dim newStartDate As DateTime = start
				Dim newEndDate As DateTime = [end]

				If Not CheckTimesharing(newStartDate, newEndDate, channelMap.Start, channelMap.[End]) Then
					Dim progChannel As Channel = Nothing
					For Each MpChannel As Channel In allChannels
						If MpChannel.IdChannel = channelMap.IdChannel Then
							progChannel = MpChannel
							Exit For
						End If
					Next
					Dim OnAirDate As DateTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value
					If [date].Length > 0 AndAlso [date] <> "-" Then
						Try
							OnAirDate = DateTime.Parse([String].Format("01.01.{0} 00:00:00", [date]))
						Catch generatedExceptionName As Exception
							Log.Info("TVMovie: Invalid year for OnAirDate - {0}", [date])
						End Try
					End If

					Dim shortDescription As String = KurzBeschreibung
					Dim description As String
					If _useShortProgramDesc Then
						description = shortDescription
					Else
						description = Beschreibung
						' If short desc has info but "long" desc has not
						If description.Length < shortDescription.Length Then
							description = shortDescription
						End If
					End If

					description = description.Replace("<br>", vbLf)

					If _extendDescription Then
						Dim sb As New StringBuilder()
						' Avoid duplicate episode title
						If (episode <> [String].Empty) AndAlso (episode <> Sendung) Then
							If Not [String].IsNullOrEmpty(duration) Then
								sb.AppendFormat("Folge: {0} ({1})" & vbLf, episode, duration)
							Else
								sb.AppendFormat("Folge: {0}" & vbLf, episode)
							End If
						End If

						If Not [String].IsNullOrEmpty([date]) Then
							If Not [String].IsNullOrEmpty(origin) Then
								sb.AppendFormat("Aus: {0} {1}" & vbLf, origin, [date])
							Else
								sb.AppendFormat("Jahr: {0}" & vbLf, [date])
							End If
						End If

						If starRating <> -1 AndAlso _showRatings Then
							'sb.Append("Wertung: " + string.Format("{0}/5", starRating) + "\n");
							sb.Append("Wertung: ")
							If shortCritic.Length > 1 Then
								sb.Append(shortCritic)
								sb.Append(" - ")
							End If
							sb.Append(BuildRatingDescription(starRating))
							If detailedRating.Length > 0 Then
								sb.Append(BuildDetailedRatingDescription(detailedRating))
							End If
						End If
						If Not [String].IsNullOrEmpty(description) Then
							sb.Append(description)
							sb.Append(vbLf)
						End If
						If director.Length > 0 Then
							sb.AppendFormat("Regie: {0}" & vbLf, director)
						End If
						If actors.Length > 0 Then
							sb.Append(BuildActorsDescription(actors))
						End If
						If Not [String].IsNullOrEmpty(classification) AndAlso classification <> "0" Then
							sb.AppendFormat("FSK: {0}" & vbLf, classification)
						End If

						description = sb.ToString()
					Else
						If _showRatings Then
							If shortCritic.Length > 1 Then
								description = shortCritic & vbLf & description
							End If
						End If
					End If

					If _showAudioFormat Then
						audioFormat = BuildAudioDescription(Convert.ToBoolean(Audiodescription), Convert.ToBoolean(DolbyDigital), Convert.ToBoolean(DolbySuround), Convert.ToBoolean(Dolby), Convert.ToBoolean(Stereo), Convert.ToBoolean(Zweikanalton))

						If Not [String].IsNullOrEmpty(audioFormat) Then
							description += "Ton: " & audioFormat
						End If
					End If

					Dim prog As New Program(progChannel.IdChannel, newStartDate, newEndDate, title, description, genre__4, _
						Program.ProgramState.None, OnAirDate, [String].Empty, [String].Empty, episode, [String].Empty, _
						EPGStarRating, classification, parentalRating)

					_tvmEpgProgs.Add(prog)

					If _slowImport AndAlso aCounter Mod 2 = 0 Then
						Thread.Sleep(20)
					End If
				End If
			Next
		End Sub

		''' <summary>
		''' Retrieve all channel-mappings from TvMovieMapping table
		''' </summary>
		''' <returns></returns>
		Private Function GetMappingList() As List(Of Mapping)
			Dim mappingList As New List(Of Mapping)()
			Try
				Dim mappingDb As IList(Of TvMovieMapping) = TvMovieMapping.ListAll()
				For Each mapping As TvMovieMapping In mappingDb
					Try
						Dim newStart As String = mapping.TimeSharingStart
						Dim newEnd As String = mapping.TimeSharingEnd
						Dim newStation As String = mapping.StationName
						Dim newChannel As String = Channel.Retrieve(mapping.IdChannel).DisplayName
						Dim newIdChannel As Integer = mapping.IdChannel

						mappingList.Add(New Mapping(newChannel, newIdChannel, newStation, newStart, newEnd))
					Catch generatedExceptionName As Exception
						Log.Info("TVMovie: Error loading mappings - make sure tv channel: {0} (ID: {1}) still exists!", mapping.StationName, mapping.IdChannel)
					End Try
				Next
			Catch ex As Exception
				Log.Info("TVMovie: Error in GetMappingList - {0}" & vbLf & "{1}", ex.Message, ex.StackTrace)
			End Try
			Return mappingList
		End Function

		''' <summary>
		''' Determines whether an entry is valid for a timesharing station 
		''' </summary>
		Private Function CheckTimesharing(ByRef progStart As DateTime, ByRef progEnd As DateTime, timeSharingStart As system.timespan, timeSharingEnd As system.timespan) As Boolean
			If timeSharingStart = timeSharingEnd Then
				Return False
			End If

			Dim stationStart As DateTime = progStart.[Date] + timeSharingStart
			Dim stationEnd As DateTime = progStart.[Date] + timeSharingEnd

			If stationStart > progStart AndAlso progEnd <= stationStart Then
				stationStart = stationStart.AddDays(-1)
			ElseIf timeSharingEnd < timeSharingStart Then
				stationEnd = stationEnd.AddDays(1)
			End If

			If progStart >= stationStart AndAlso progStart < stationEnd AndAlso progEnd > stationEnd Then
				progEnd = stationEnd
			End If

			If progStart <= stationStart AndAlso progEnd > stationStart AndAlso progEnd < stationEnd Then
				progStart = stationStart
			End If

			If (progEnd <= stationEnd) AndAlso (progStart >= stationStart) Then
				Return False
			End If

			Return True
		End Function

		''' <summary>
		''' passing the TV movie sound bool params this method returns the audio format as string
		''' </summary>
		''' <param name="audioDesc"></param>
		''' <param name="dolbyDigital"></param>
		''' <param name="dolbySuround"></param>
		''' <param name="dolby"></param>
		''' <param name="stereo"></param>
		''' <param name="dualAudio"></param>
		''' <returns>plain text audio format</returns>
		Private Function BuildAudioDescription(audioDesc As Boolean, dolbyDigital As Boolean, dolbySurround As Boolean, dolby As Boolean, stereo As Boolean, dualAudio As Boolean) As String
			Dim sb As New StringBuilder()
			If dolbyDigital Then
				sb.AppendLine("Dolby Digital")
			End If
			If dolbySurround Then
				sb.AppendLine("Dolby Surround")
			End If
			If dolby Then
				sb.AppendLine("Dolby")
			End If
			If stereo Then
				sb.AppendLine("Stereo")
			End If
			If dualAudio Then
				sb.AppendLine("Mehrkanal-Ton")
			End If

			Dim result As String = sb.ToString().Replace(Environment.NewLine, ",")
			' Remove trailing comma
			If result.EndsWith(",") Then
				result = result.Remove(result.Length - 1)
			End If
			Return result
		End Function

		''' <summary>
		''' Translates the numeric db values for rating into readable text
		''' </summary>
		''' <param name="dbRating"></param>
		''' <returns>One word indicating the rating</returns>
		Private Function BuildRatingDescription(dbRating As Integer) As String
			Dim TVMovieRating As String = [String].Empty

			Select Case dbRating
				Case 0
					TVMovieRating = "uninteressant"
					Exit Select
				Case 1
					TVMovieRating = "durchschnittlich"
					Exit Select
				Case 2
					TVMovieRating = "empfehlenswert"
					Exit Select
				Case 3
					TVMovieRating = "Tages-Tipp!"
					Exit Select
				Case 4
					TVMovieRating = "Blockbuster!"
					Exit Select
				Case 5
					TVMovieRating = "Genre-Tipp"
					Exit Select
				Case 6
					TVMovieRating = "Genre-Highlight!"
					Exit Select
				Case Else
					TVMovieRating = "---"
					Exit Select
			End Select

			Return TVMovieRating & vbLf
		End Function

		''' <summary>
		''' Formats the db rating into nice text
		''' </summary>
		''' <param name="dbDetailedRating"></param>
		''' <returns></returns>
		Private Function BuildDetailedRatingDescription(dbDetailedRating As String) As String
			' "Spaß=1;Action=3;Erotik=1;Spannung=3;Anspruch=0"
			Dim posidx As Integer = 0
			Dim detailedRating As String = [String].Empty
			Dim strb As New StringBuilder()

			If dbDetailedRating <> [String].Empty Then
				posidx = dbDetailedRating.IndexOf("Spaß=")
				If posidx > 0 Then
					If dbDetailedRating(posidx + 5) <> "0"C Then
						strb.Append(dbDetailedRating.Substring(posidx, 6))
						strb.Append(vbLf)
					End If
				End If
				posidx = dbDetailedRating.IndexOf("Action=")
				If posidx > 0 Then
					If dbDetailedRating(posidx + 7) <> "0"C Then
						strb.Append(dbDetailedRating.Substring(posidx, 8))
						strb.Append(vbLf)
					End If
				End If
				posidx = dbDetailedRating.IndexOf("Erotik=")
				If posidx > 0 Then
					If dbDetailedRating(posidx + 7) <> "0"C Then
						strb.Append(dbDetailedRating.Substring(posidx, 8))
						strb.Append(vbLf)
					End If
				End If
				posidx = dbDetailedRating.IndexOf("Spannung=")
				If posidx > 0 Then
					If dbDetailedRating(posidx + 9) <> "0"C Then
						strb.Append(dbDetailedRating.Substring(posidx, 10))
						strb.Append(vbLf)
					End If
				End If
				posidx = dbDetailedRating.IndexOf("Anspruch=")
				If posidx > 0 Then
					If dbDetailedRating(posidx + 9) <> "0"C Then
						strb.Append(dbDetailedRating.Substring(posidx, 10))
						strb.Append(vbLf)
					End If
				End If
				posidx = dbDetailedRating.IndexOf("Gefühl=")
				If posidx > 0 Then
					If dbDetailedRating(posidx + 7) <> "0"C Then
						strb.Append(dbDetailedRating.Substring(posidx, 8))
						strb.Append(vbLf)
					End If
				End If
				detailedRating = strb.ToString()
			End If

			Return detailedRating
		End Function

		''' <summary>
		''' Formats the actors text into a string suitable for the description field
		''' </summary>
		''' <param name="dbActors"></param>
		''' <returns></returns>
		Private Function BuildActorsDescription(dbActors As String) As String
			Dim strb As New StringBuilder()
			' Mit: Bernd Schramm (Buster der Hund);Sandra Schwarzhaupt (Gwendolyn die Katze);Joachim Kemmer (Tortellini der Hahn);Mario Adorf (Fred der Esel);Katharina Thalbach (die Erbin);Peer Augustinski (Dr. Gier);Klausjürgen Wussow (Der Erzähler);Hartmut Engler (Hund Buster);Bert Henry (Drehbuch);Georg Reichel (Drehbuch);Dagmar Kekule (Drehbuch);Peter Wolf (Musik);Dagmar Kekulé (Drehbuch)
			strb.Append("Mit: ")
			If _actorCount < 1 Then
				strb.Append(dbActors)
				strb.Append(vbLf)
			Else
				Dim splitActors As String() = dbActors.Split(";"C)
				If splitActors IsNot Nothing AndAlso splitActors.Length > 0 Then
					For i As Integer = 0 To splitActors.Length - 1
						If i < _actorCount Then
							strb.Append(splitActors(i))
							strb.Append(vbLf)
						Else
							Exit For
						End If
					Next
				End If
			End If

			Return strb.ToString()
		End Function

		Private Function datetolong(dt As DateTime) As Long
			Try
				Dim iSec As Long = 0
				'(long)dt.Second;
				Dim iMin As Long = CLng(dt.Minute)
				Dim iHour As Long = CLng(dt.Hour)
				Dim iDay As Long = CLng(dt.Day)
				Dim iMonth As Long = CLng(dt.Month)
				Dim iYear As Long = CLng(dt.Year)

				Dim lRet As Long = (iYear)
				lRet = lRet * 100L + iMonth
				lRet = lRet * 100L + iDay
				lRet = lRet * 100L + iHour
				lRet = lRet * 100L + iMin
				lRet = lRet * 100L + iSec
				Return lRet
			Catch generatedExceptionName As Exception
			End Try
			Return 0
		End Function

		''' <summary>
		''' Launches TV Movie's own internet update tool
		''' </summary>
		''' <returns>Number of seconds needed for the update</returns>
		Public Function LaunchTVMUpdater(aHideUpdater As Boolean) As Long
			Dim UpdateDuration As Long = 0
			Dim UpdaterPath As String = Path.Combine(TvMovie.TVMovieProgramPath, "tvuptodate.exe")
			If File.Exists(UpdaterPath) Then
				Dim BenchClock As New Stopwatch()

				Try
					BenchClock.Start()

					' check whether e.g. tv movie itself already started an update
					Dim processes As Process() = Process.GetProcessesByName("tvuptodate")
					If processes.Length > 0 Then
						processes(0).WaitForExit(1200000)
						BenchClock.[Stop]()
						UpdateDuration = (BenchClock.ElapsedMilliseconds \ 1000)
						Log.Info("TVMovie: tvuptodate was already running - waited {0} seconds for internet update to finish", Convert.ToString(UpdateDuration))
						Return UpdateDuration
					End If

					Dim startInfo As New ProcessStartInfo("tvuptodate.exe")
					If aHideUpdater Then
						startInfo.Arguments = "/hidden"
					End If
					startInfo.FileName = UpdaterPath
					' replaced with startInfo.Arguments = "/hidden" | flokel | 11.01.09
					' startInfo.WindowStyle = aHideUpdater ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;
					startInfo.WorkingDirectory = Path.GetDirectoryName(UpdaterPath)
					Dim UpdateProcess As Process = Process.Start(startInfo)
					UpdateProcess.PriorityClass = ProcessPriorityClass.BelowNormal

					UpdateProcess.WaitForExit(1200000)
					' do not wait longer than 20 minutes for the internet update
					BenchClock.[Stop]()
					UpdateDuration = (BenchClock.ElapsedMilliseconds \ 1000)
					Log.Info("TVMovie: tvuptodate finished internet update in {0} seconds", Convert.ToString(UpdateDuration))
				Catch ex As Exception
					BenchClock.[Stop]()
					UpdateDuration = (BenchClock.ElapsedMilliseconds \ 1000)
					Log.Info("TVMovie: LaunchTVMUpdater failed: {0}", ex.Message)
				End Try
			Else
				Log.Info("TVMovie: tvuptodate.exe not found in default location: {0}", UpdaterPath)
					' workaround for systems without tvuptodate
				UpdateDuration = 30
			End If

			Return UpdateDuration
		End Function

		#End Region
	End Class

	' class
End Namespace
