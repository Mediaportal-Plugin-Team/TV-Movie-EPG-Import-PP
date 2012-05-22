#Region "Copyright (C) 2005-2011 Team MediaPortal"

' Copyright (C) 2005-2011 Team MediaPortal
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

Imports System
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
Imports System.Collections
Imports Gentle.Framework
Imports Gentle.Common
Imports TvMovie.TvDatabase
Imports TvMovie.ClickfinderDB


Namespace TvEngine

#Region "TVMChannel struct"

    Public Structure TVMChannel
        Private fID As String
        Private fSenderKennung As String
        Private fBezeichnung As String
        Private fWebseite As String
        Private fSortNrTVMovie As String
        Private fZeichen As String

        Public Sub New(ByVal aID As String, ByVal aSenderKennung As String, ByVal aBezeichnung As String, ByVal aWebseite As String, ByVal aSortNrTVMovie As String, ByVal aZeichen As String)
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

        Private _ImportStartTime As Date
#End Region

#Region "Events"

        Public Delegate Sub ProgramsChanged(ByVal value As Integer, ByVal maximum As Integer, ByVal text As String)

        'public event ProgramsChanged OnProgramsChanged;
        Public Delegate Sub StationsChanged(ByVal value As Integer, ByVal maximum As Integer, ByVal text As String)

        Public Event OnStationsChanged As StationsChanged

#End Region

#Region "Mapping struct"

        Private Structure Mapping
            Private _mpChannel As String
            Private _tvmEpgChannel As String
            Private _mpIdChannel As Integer
            Private _start As System.TimeSpan
            Private _end As System.TimeSpan

            Public Sub New(ByVal mpChannel As String, ByVal mpIdChannel As Integer, ByVal tvmChannel As String, ByVal start As String, ByVal [end] As String)
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

            Public ReadOnly Property Start() As System.TimeSpan
                Get
                    Return _start
                End Get
            End Property

            Public ReadOnly Property [End]() As System.TimeSpan
                Get
                    Return _end
                End Get
            End Property

            Private Shared Function CleanInput(ByVal input As String) As System.TimeSpan
                Dim hours As Integer = 0
                Dim minutes As Integer = 0
                input = input.Trim()
                Dim index As Integer = input.IndexOf(":"c)
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

                Return New System.TimeSpan(hours, minutes, 0)
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
            Set(ByVal value As Boolean)
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
                MyLog.Info("TVMovie: Exception in GetChannels: {0}" & vbLf & "{1}", ex.Message, ex.StackTrace)
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
                MyLog.Info("TVMovie: Exception creating OleDbConnection: {0}", connex.Message)
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
                            MyLog.Info("TVMovie: Exception filling Sender DataSet - {0}" & vbLf & "{1}", dsex.Message, dsex.StackTrace)
                            Return False
                        End Try
                    End Using
                End Using
            Catch ex As System.Data.OleDb.OleDbException
                MyLog.Info("TVMovie: Error accessing TV Movie Clickfinder database while reading stations: {0}", ex.Message)
                MyLog.Info("TVMovie: Exception: {0}", ex.StackTrace)
                _canceled = True
                Return False
            Catch ex2 As Exception
                MyLog.Info("TVMovie: Exception: {0}, {1}", ex2.Message, ex2.StackTrace)
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
                MyLog.Info("TVMovie: Exception: {0}, {1}", ex.Message, ex.StackTrace)
            End Try

            _channelList = GetChannels()
            Return True
        End Function

        Public ReadOnly Property NeedsImport() As Boolean
            Get
                Try
                    Dim restTime As New System.TimeSpan(Convert.ToInt32(TvBLayer.GetSetting("TvMovieRestPeriod", "24").Value), 0, 0)
                    Dim lastUpdated As DateTime = Convert.ToDateTime(TvBLayer.GetSetting("TvMovieLastUpdate", "0").Value)
                    '        if (Convert.ToInt64(TvBLayer.GetSetting("TvMovieLastUpdate", "0").Value) == LastUpdate)
                    If lastUpdated >= (DateTime.Now - restTime) Then
                        Return False
                    Else
                        MyLog.Debug("TVMovie: Last update was at {0} - new import scheduled", Convert.ToString(lastUpdated))
                        Return True
                    End If
                Catch ex As Exception
                    MyLog.Info("TVMovie: An error occured checking the last import time {0}", ex.Message)
                    MyLog.Write(ex)
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

            'TvMovie++: In Setting speichern das Import gerade läuft
            Dim setting As Setting = TvBLayer.GetSetting("TvMovieImportIsRunning", "false")
            setting.Value = "true"
            setting.Persist()


            Dim mappingList As List(Of Mapping) = GetMappingList()
            If mappingList Is Nothing OrElse mappingList.Count < 1 Then
                MyLog.Info("TVMovie: Cannot import from TV Movie database - no mappings found")
                'TvMovie++: In Setting speichern das Import beendet ist
                setting = TvBLayer.GetSetting("TvMovieImportIsRunning", "false")
                setting.Value = "false"
                setting.Persist()
                Return
            End If

            Dim ImportStartTime As DateTime = DateTime.Now
            _ImportStartTime = ImportStartTime
            MyLog.Debug("TVMovie: Importing database")
            Dim maximum As Integer = 0

            For Each tvmChan As TVMChannel In _tvmEpgChannels
                For Each mapping As Mapping In mappingList
                    If mapping.TvmEpgChannel = tvmChan.TvmEpgChannel Then
                        maximum += 1
                        Exit For
                    End If
                Next
            Next

            MyLog.Debug("TVMovie: Calculating stations done")

            ' setting update time of epg import to avoid that the background thread triggers another import
            ' if the process lasts longer than the timer's update check interval
            setting = TvBLayer.GetSetting("TvMovieLastUpdate")
            setting.Value = DateTime.Now.ToString()
            setting.Persist()

            MyLog.Debug("TVMovie: Mapped {0} stations for EPG import", Convert.ToString(maximum))
            Dim counter As Integer = 0

            _tvmEpgProgs.Clear()

            ' get all tv channels from MP DB via gentle.net
            Dim allChannels As IList(Of Channel) = Channel.ListAll()

            For Each station As TVMChannel In _tvmEpgChannels
                If _canceled Then
                    Return
                End If

                MyLog.Info("TVMovie: Searching time share mappings for station: {0}", station.TvmEpgDescription)
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

                        MyLog.Info("TVMovie: Importing {3} time frame(s) for MP channel [{0}/{1}] - {2}", Convert.ToString(counter), Convert.ToString(maximum), display, Convert.ToString(channelNames.Count))

                        _tvmEpgProgs.Clear()

                        _programsCounter += ImportStation(station.TvmEpgChannel, channelNames, allChannels)

                        Dim importPrio As ThreadPriority = If(_slowImport, ThreadPriority.BelowNormal, ThreadPriority.AboveNormal)
                        If _slowImport Then
                            Thread.Sleep(32)
                        End If

                        ' make a copy of this list because Insert it done in syncronized threads - therefore the object reference would cause multiple/missing entries
                        Dim InsertCopy As New List(Of Program)(_tvmEpgProgs)
                        Dim debugCount As Integer = TvBLayer.InsertPrograms(InsertCopy, DeleteBeforeImportOption.OverlappingPrograms, importPrio)
                        MyLog.Info("TVMovie: Inserted {0} programs", debugCount)

                    Catch ex As Exception
                        MyLog.Info("TVMovie: Error inserting programs - {0}", ex.StackTrace)
                    End Try
                End If
            Next

            MyLog.Debug("TVMovie: Waiting for database to be updated...")
            TvBLayer.WaitForInsertPrograms()
            MyLog.Debug("TVMovie: Database update finished.")


            RaiseEvent OnStationsChanged(maximum, maximum, "Import done")

            If Not _canceled Then
                Try
                    setting = TvBLayer.GetSetting("TvMovieLastUpdate")
                    setting.Value = DateTime.Now.ToString()
                    setting.Persist()

                    Dim ImportDuration As System.TimeSpan = (DateTime.Now - ImportStartTime)
                    MyLog.Debug("TVMovie: Imported {0} database entries for {1} stations in {2} seconds", _programsCounter, counter, Convert.ToString(ImportDuration.TotalSeconds))

                Catch generatedExceptionName As Exception
                    MyLog.Error("TVMovie: Error updating the database with last import date")
                End Try

                'TV Movie++ Enhancement by Scrounger
                StartTVMoviePlus()

                If _tvbLayer.GetSetting("TvMovieIsEpisodenScanner", "false").Value = "false" Then
                    MyLog.Info("TVMovie: overall Import duration: {0}", (Date.Now - ImportStartTime).Minutes & "min " & (Date.Now - ImportStartTime).Seconds & "s")
                End If

                'TvMovie++: In Setting speichern das Import beendet ist
                setting = TvBLayer.GetSetting("TvMovieImportIsRunning", "false")
                setting.Value = "false"
                setting.Persist()

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

        Private Function ImportStation(ByVal stationName As String, ByVal channelNames As List(Of Mapping), ByVal allChannels As IList(Of Channel)) As Integer
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

            Dim importTime As DateTime = DateTime.Now.Subtract(System.TimeSpan.FromHours(4))
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
                    MyLog.Info("TVMovie: Error accessing TV Movie Clickfinder database - import of current station canceled")
                    MyLog.[Error]("TVMovie: Exception: {0}", ex)
                    Return 0
                Catch ex1 As Exception
                    Try
                        databaseTransaction.Rollback()
                    Catch generatedExceptionName As Exception
                    End Try
                    MyLog.Info("TVMovie: Exception: {0}", ex1)
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
        Private Sub ImportSingleChannelData(ByVal channelNames As List(Of Mapping), ByVal allChannels As IList(Of Channel), ByVal aCounter As Integer, ByVal SenderKennung As String, ByVal Beginn As String, ByVal Ende As String, _
         ByVal Sendung As String, ByVal Genre__1 As String, ByVal Kurzkritik As String, ByVal KurzBeschreibung As String, ByVal Beschreibung As String, ByVal Audiodescription As String, _
         ByVal DolbySuround As String, ByVal Stereo As String, ByVal DolbyDigital As String, ByVal Dolby As String, ByVal Zweikanalton As String, ByVal FSK As String, _
         ByVal Herstellungsjahr As String, ByVal Originaltitel As String, ByVal Regie As String, ByVal Darsteller As String, ByVal Interessant As String, ByVal Bewertungen As String, _
         ByVal Live__2 As String, ByVal Dauer As String, ByVal Herstellungsland As String, ByVal Wiederholung As String)
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
            'Dim Rating As String = [String].Empty
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
                MyLog.Info("TVMovie: Error parsing EPG time data - {0}", ex2.ToString())
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

            'detailedRating

            Dim EPGStarRating As Short = -1
            'Rating = Bewertungen
            'If Rating.Length > 0 Then
            '    Dim tmp As String = Replace(Replace(Replace(Replace(Replace(Replace(Replace(Bewertungen, ";", " "), "Spaß=", ""), "Action=", ""), "Erotik=", ""), "Spannung=", ""), "Gefühl=", ""), "Anspruch=", "")
            '    For d As Integer = 1 To tmp.Length
            '        If IsNumeric(Mid$(tmp, d, 1)) Then
            '            EPGStarRating = EPGStarRating + CShort(Mid$(tmp, d, 1))
            '        End If
            '    Next
            'End If

            If Bewertungen.Length > 0 Then
                Dim RatingString As String = Bewertungen
                Dim NewList As New List(Of String)
                Dim KeyList As New Generic.Dictionary(Of String, String)
                Dim Rating As Short = 0

                'Split the String by ";"
                NewList.AddRange(Split(RatingString, ";"))

                'Remove double entries
                For i As Integer = 0 To NewList.Count - 1
                    If KeyList.ContainsKey(NewList(i)) = False Then
                        KeyList.Add(NewList(i), String.Empty)
                    End If
                Next

                'Calculate rating
                For Each item As KeyValuePair(Of String, String) In KeyList
                    Rating = Rating + CShort(Replace(Replace(Replace(Replace(Replace(Replace(item.Key, "Spaß=", ""), "Action=", ""), "Erotik=", ""), "Spannung=", ""), "Gefühl=", ""), "Anspruch=", ""))
                Next
                If Rating > 10 Then
                    EPGStarRating = 10
                Else
                    EPGStarRating = Rating
                End If
            End If

            'Select Case starRating
            '    Case 0
            '        EPGStarRating = 2
            '        Exit Select
            '    Case 1
            '        EPGStarRating = 4
            '        Exit Select
            '    Case 2
            '        EPGStarRating = 6
            '        Exit Select
            '    Case 3
            '        EPGStarRating = 8
            '        Exit Select
            '    Case 4
            '        EPGStarRating = 10
            '        Exit Select
            '    Case 5
            '        EPGStarRating = 8
            '        Exit Select
            '    Case 6
            '        EPGStarRating = 10
            '        Exit Select
            '    Case Else
            '        EPGStarRating = -1
            '        Exit Select
            'End Select

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
                            MyLog.Info("TVMovie: Invalid year for OnAirDate - {0}", [date])
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
                        MyLog.Info("TVMovie: Error loading mappings - make sure tv channel: {0} (ID: {1}) still exists!", mapping.StationName, mapping.IdChannel)
                    End Try
                Next
            Catch ex As Exception
                MyLog.Info("TVMovie: Error in GetMappingList - {0}" & vbLf & "{1}", ex.Message, ex.StackTrace)
            End Try
            Return mappingList
        End Function

        ''' <summary>
        ''' Determines whether an entry is valid for a timesharing station 
        ''' </summary>
        Private Function CheckTimesharing(ByRef progStart As DateTime, ByRef progEnd As DateTime, ByVal timeSharingStart As System.TimeSpan, ByVal timeSharingEnd As System.TimeSpan) As Boolean
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
        Private Function BuildAudioDescription(ByVal audioDesc As Boolean, ByVal dolbyDigital As Boolean, ByVal dolbySurround As Boolean, ByVal dolby As Boolean, ByVal stereo As Boolean, ByVal dualAudio As Boolean) As String
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
        Private Function BuildRatingDescription(ByVal dbRating As Integer) As String
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
        Private Function BuildDetailedRatingDescription(ByVal dbDetailedRating As String) As String
            ' "Spaß=1;Action=3;Erotik=1;Spannung=3;Anspruch=0"
            Dim posidx As Integer = 0
            Dim detailedRating As String = [String].Empty
            Dim strb As New StringBuilder()

            If dbDetailedRating <> [String].Empty Then
                posidx = dbDetailedRating.IndexOf("Spaß=")
                If posidx > 0 Then
                    If dbDetailedRating(posidx + 5) <> "0"c Then
                        strb.Append(dbDetailedRating.Substring(posidx, 6))
                        strb.Append(vbLf)
                    End If
                End If
                posidx = dbDetailedRating.IndexOf("Action=")
                If posidx > 0 Then
                    If dbDetailedRating(posidx + 7) <> "0"c Then
                        strb.Append(dbDetailedRating.Substring(posidx, 8))
                        strb.Append(vbLf)
                    End If
                End If
                posidx = dbDetailedRating.IndexOf("Erotik=")
                If posidx > 0 Then
                    If dbDetailedRating(posidx + 7) <> "0"c Then
                        strb.Append(dbDetailedRating.Substring(posidx, 8))
                        strb.Append(vbLf)
                    End If
                End If
                posidx = dbDetailedRating.IndexOf("Spannung=")
                If posidx > 0 Then
                    If dbDetailedRating(posidx + 9) <> "0"c Then
                        strb.Append(dbDetailedRating.Substring(posidx, 10))
                        strb.Append(vbLf)
                    End If
                End If
                posidx = dbDetailedRating.IndexOf("Anspruch=")
                If posidx > 0 Then
                    If dbDetailedRating(posidx + 9) <> "0"c Then
                        strb.Append(dbDetailedRating.Substring(posidx, 10))
                        strb.Append(vbLf)
                    End If
                End If
                posidx = dbDetailedRating.IndexOf("Gefühl=")
                If posidx > 0 Then
                    If dbDetailedRating(posidx + 7) <> "0"c Then
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
        Private Function BuildActorsDescription(ByVal dbActors As String) As String
            Dim strb As New StringBuilder()
            ' Mit: Bernd Schramm (Buster der Hund);Sandra Schwarzhaupt (Gwendolyn die Katze);Joachim Kemmer (Tortellini der Hahn);Mario Adorf (Fred der Esel);Katharina Thalbach (die Erbin);Peer Augustinski (Dr. Gier);Klausjürgen Wussow (Der Erzähler);Hartmut Engler (Hund Buster);Bert Henry (Drehbuch);Georg Reichel (Drehbuch);Dagmar Kekule (Drehbuch);Peter Wolf (Musik);Dagmar Kekulé (Drehbuch)
            strb.Append("Mit: ")
            If _actorCount < 1 Then
                strb.Append(dbActors)
                strb.Append(vbLf)
            Else
                Dim splitActors As String() = dbActors.Split(";"c)
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

        Private Function datetolong(ByVal dt As DateTime) As Long
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
        Public Function LaunchTVMUpdater(ByVal aHideUpdater As Boolean) As Long
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
                        MyLog.Info("TVMovie: tvuptodate was already running - waited {0} seconds for internet update to finish", Convert.ToString(UpdateDuration))
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
                    MyLog.Info("TVMovie: tvuptodate finished internet update in {0} seconds", Convert.ToString(UpdateDuration))
                Catch ex As Exception
                    BenchClock.[Stop]()
                    UpdateDuration = (BenchClock.ElapsedMilliseconds \ 1000)
                    MyLog.Info("TVMovie: LaunchTVMUpdater failed: {0}", ex.Message)
                End Try
            Else
                MyLog.Info("TVMovie: tvuptodate.exe not found in default location: {0}", UpdaterPath)
                ' workaround for systems without tvuptodate
                UpdateDuration = 30
            End If

            Return UpdateDuration
        End Function

#End Region

#Region "TVMovie++ enhancement"

        Private Sub StartTVMoviePlus()
            Try

                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieRunAppAfter = " & _tvbLayer.GetSetting("TvMovieRunAppAfter").Value)
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieImportTvSeriesInfos = " & _tvbLayer.GetSetting("TvMovieImportTvSeriesInfos").Value)
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieImportMovingPicturesInfos = " & _tvbLayer.GetSetting("TvMovieImportMovingPicturesInfos").Value)
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: ClickfinderDataAvailable = " & _tvbLayer.GetSetting("ClickfinderDataAvailable").Value)
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: ClickfinderDataAvailable = " & _tvbLayer.GetSetting("TvMovieIsEpisodenScanner").Value)

                Try
                    Broker.Execute("DROP TABLE mptvdb.tvmovieprogram")
                    Broker.Execute("CREATE TABLE mptvdb.TVMovieProgram ( idTVMovieProgram INT NOT NULL AUTO_INCREMENT , idProgram INT NOT NULL DEFAULT 0 , TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT(1) NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , needsUpdate BIT(1) NOT NULL DEFAULT 1 , Dolby BIT(1) NOT NULL DEFAULT 0 , HDTV BIT(1) NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Year DATETIME NOT NULL , Describtion TEXT , ShortDescribtion TEXT , PRIMARY KEY (idTVMovieProgram) )")
                Catch ex As Exception
                    'Falls die Tabelle nicht existiert, abfangen & erstellen
                    Broker.Execute("CREATE TABLE mptvdb.TVMovieProgram ( idTVMovieProgram INT NOT NULL AUTO_INCREMENT , idProgram INT NOT NULL DEFAULT 0 , TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT(1) NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , needsUpdate BIT(1) NOT NULL DEFAULT 1 , Dolby BIT(1) NOT NULL DEFAULT 0 , HDTV BIT(1) NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Year DATETIME NOT NULL , Describtion TEXT , ShortDescribtion TEXT , PRIMARY KEY (idTVMovieProgram) )")
                End Try

                Try
                    'Table TvMovieSeriesMapping anlegen
                    Broker.Execute("CREATE  TABLE `mptvdb`.`TvMovieSeriesMapping` ( `idSeries` INT NOT NULL , `EpgTitle` VARCHAR(255) , PRIMARY KEY (`idSeries`) )")
                    MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieSeriesMapping table created")
                Catch ex As Exception
                    'existiert bereits
                    MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieSeriesMapping table exist")
                End Try

                'TVSeries importieren
                If _tvbLayer.GetSetting("TvMovieImportTvSeriesInfos").Value = "true" Then
                    GetSeriesInfos()
                End If

                'MovingPictures importieren
                If _tvbLayer.GetSetting("TvMovieImportMovingPicturesInfos").Value = "true" Then
                    GetMovingPicturesInfos()
                End If

                'MP VideoDatabase importieren
                If _tvbLayer.GetSetting("TvMovieImportVideoDatabaseInfos").Value = "true" Then
                    GetVideoDatabaseInfos()
                End If

                'Tv Movie Highlight und Suchoptionen für Clickfinder ProgramGuide importieren
                If _tvbLayer.GetSetting("ClickfinderDataAvailable").Value = "true" Then
                    GetTvMovieHighlights()
                End If

                'Run App after import
                If _tvbLayer.GetSetting("TvMovieRunAppAfter").Value IsNot String.Empty Then

                    Dim _Thread As New Thread(AddressOf RunApplicationAfterImport)
                    _Thread.Start()

                End If

                'Am Ende nochmal TvMovieLastUpdate in Settings speichern -> Abschlusszeit
                Dim setting As Setting = TvBLayer.GetSetting("TvMovieLastUpdate")
                setting.Value = DateTime.Now.ToString()
                setting.Persist()

            Catch ex As Exception
                MyLog.Error("TVMovie: [StartTVMoviePlus]: error: {0} stack: {1}", ex.Message, ex.StackTrace)
            End Try
        End Sub

        Private Sub GetSeriesInfos()
            Try
                MyLog.[Debug]("TVMovie: [GetSeriesInfos]: start import")

                Dim _Counter As Integer = 0
                Dim _CounterFound As Integer = 0
                Dim _CounterNewEpisode As Integer = 0
                Dim _SQLString As String = String.Empty
                'Alle Serien aus DB laden
                Dim _TvSeriesDB As New TVSeriesDB
                _TvSeriesDB.LoadAllSeries()

                'Nach allen Serien im EPG suchen
                For i As Integer = 0 To _TvSeriesDB.CountSeries - 1

                    Try

                        Dim _EpisodeFoundCounter As Integer = 0

                        Dim _Result As New ArrayList

                        Try
                            Dim _TvMovieSeriesMapping As TvMovieSeriesMapping = TvMovieSeriesMapping.Retrieve(_TvSeriesDB(i).SeriesID)

                            If Not String.IsNullOrEmpty(_TvMovieSeriesMapping.EpgTitle) Then
                                _SQLString = _
                                            "Select idProgram from program WHERE title LIKE '" & allowedSigns(_TvMovieSeriesMapping.EpgTitle) & "'"
                                MyLog.[Info]("TVMovie: [GetSeriesInfos]: manuel mapping found: {0} -> {1}", _TvSeriesDB(i).SeriesName, _TvMovieSeriesMapping.EpgTitle)
                            Else
                                _SQLString = _
                                    "Select idProgram from program WHERE title LIKE '" & allowedSigns(_TvSeriesDB(i).SeriesName)
                                MyLog.Warn("TVMovie: [GetSeriesInfos]: EpgTitle is empty for idseries = {0}", _TvSeriesDB(i).SeriesID)
                            End If

                        Catch ex As Exception
                            _SQLString = _
                            "Select idProgram from program WHERE title LIKE '" & allowedSigns(_TvSeriesDB(i).SeriesName) & "'"
                        End Try

                        _Result.AddRange(Broker.Execute(_SQLString).TransposeToFieldList("idProgram", False))

                        For d As Integer = 0 To _Result.Count - 1
                            Dim _program As Program = Program.Retrieve(CInt(_Result.Item(d)))
                            Try

                                'Falls Episode gefunden wurde
                                If _TvSeriesDB.EpisodeFound(_TvSeriesDB(i).SeriesID, allowedSigns(_program.EpisodeName)) = True Then

                                    'Daten im EPG (program) updaten
                                    _program.SeriesNum = CStr(_TvSeriesDB.SeasonIndex)
                                    _program.EpisodeNum = CStr(_TvSeriesDB.EpisodeIndex)
                                    _program.StarRating = _TvSeriesDB.EpisodeRating

                                    'Falls Episode nicht lokal verfügbar -> im EPG Describtion kennzeichnen
                                    If _TvSeriesDB.EpisodeExistLocal = False Then
                                        If InStr(_program.Description, "Neue Folge: " & _program.EpisodeName) = 0 Then
                                            _program.Description = Replace(_program.Description, "Folge: " & _program.EpisodeName, "Neue Folge: " & _program.EpisodeName)
                                        End If
                                        _CounterNewEpisode = _CounterNewEpisode + 1
                                    End If

                                    _program.Persist()

                                    'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                                    If _tvbLayer.GetSetting("ClickfinderDataAvailable", "false").Value = "true" Then

                                        'idProgram in TvMovieProgram suchen & Daten aktualisieren
                                        Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_program.IdProgram)
                                        _TvMovieProgram.idSeries = _TvSeriesDB(i).SeriesID
                                        _TvMovieProgram.idEpisode = _TvSeriesDB.EpisodeCompositeID
                                        _TvMovieProgram.local = True

                                        'Episoden Image
                                        If Not String.IsNullOrEmpty(_TvSeriesDB.EpisodeImage) = True Then
                                            _TvMovieProgram.EpisodeImage = _TvSeriesDB.EpisodeImage
                                        End If

                                        'Serien Poster Image
                                        If Not String.IsNullOrEmpty(_TvSeriesDB(i).SeriesPosterImage) = True Then
                                            _TvMovieProgram.SeriesPosterImage = _TvSeriesDB(i).SeriesPosterImage
                                        End If

                                        'FanArt
                                        If Not String.IsNullOrEmpty(_TvSeriesDB(i).FanArt) = True Then
                                            _TvMovieProgram.FanArt = _TvSeriesDB(i).FanArt
                                        End If

                                        If _TvSeriesDB.EpisodeExistLocal = False Then
                                            _TvMovieProgram.local = False
                                        End If

                                        _TvMovieProgram.needsUpdate = True
                                        _TvMovieProgram.Persist()

                                    End If

                                    _EpisodeFoundCounter = _EpisodeFoundCounter + 1

                                End If

                            Catch ex As Exception
                                MyLog.[Error]("TVMovie: [GetSeriesInfos]: Loop :Result exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                                MyLog.[Error]("TVMovie: [GetSeriesInfos]: title:{0} idchannel:{1} startTime: {2}", _program.Title, _program.ReferencedChannel.DisplayName, _program.StartTime)
                            End Try
                        Next

                        MyLog.[Info]("TVMovie: [GetSeriesInfos]: {0}: {1}/{2} episodes found", _TvSeriesDB(i).SeriesName, _EpisodeFoundCounter, _Result.Count)
                        _CounterFound = _CounterFound + _EpisodeFoundCounter
                        _Counter = _Counter + _Result.Count

                    Catch ex As Exception
                        MyLog.[Error]("TVMovie: [GetSeriesInfos]: Loop _TvSeriesDB - exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                    End Try
                Next

                _TvSeriesDB.Dispose()

                MyLog.Info("")
                MyLog.[Info]("TVMovie: [GetSeriesInfos]: Summary: Infos for {0}/{1} episodes found, {2} new episodes identify", _CounterFound, _Counter, _CounterNewEpisode)

            Catch ex As Exception
                MyLog.[Error]("TVMovie: [GetSeriesInfos]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            End Try

        End Sub


        Private Sub GetMovingPicturesInfos()
            Try
                MyLog.[Debug]("TVMovie: [GetMovingPicturesInfos]: start import")

                Dim _SQLString As String = String.Empty
                Dim _MovingPicturesDB As New MovingPicturesDB
                _MovingPicturesDB.LoadAllMovingPicturesFilms()

                Dim EPGCounter As Integer = 0
                Dim MovieCounter As Integer = 0

                For i As Integer = 0 To _MovingPicturesDB.Count - 1

                    Try

                        Dim _Result As New ArrayList

                        'nach Mov.Pic Titel suchen
                        _SQLString = _
                            "Select idProgram from program WHERE title LIKE '" & allowedSigns(_MovingPicturesDB(i).Title) & "' "

                        'nach Mov.Pic Titel über Dateiname suchen
                        If Not String.IsNullOrEmpty(_MovingPicturesDB(i).TitlebyFilename) Then
                            _SQLString = _SQLString & _
                            "OR title LIKE '" & allowedSigns(_MovingPicturesDB(i).TitlebyFilename) & "' "
                        End If

                        'nach Mov.Pic alternateTitel 
                        If Not String.IsNullOrEmpty(_MovingPicturesDB(i).AlternateTitle) Then
                            _SQLString = _SQLString & _
                            "OR title LIKE '" & allowedSigns(_MovingPicturesDB(i).AlternateTitle) & "' "
                        End If

                        _Result.AddRange(Broker.Execute(_SQLString).TransposeToFieldList("idProgram", False))

                        If _Result.Count > 0 Then
                            MovieCounter = MovieCounter + 1
                        End If

                        For d As Integer = 0 To _Result.Count - 1

                            Dim _program As Program = Program.Retrieve(CInt(_Result.Item(d)))

                            Try

                                'Daten im EPG (program) updaten
                                _program.StarRating = _MovingPicturesDB(i).Rating
                                If InStr(_program.Description, "existiert lokal") = 0 And String.IsNullOrEmpty(_program.SeriesNum) Then
                                    _program.Description = "existiert lokal" & vbNewLine & _program.Description
                                End If

                                _program.Persist()

                                'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                                If _tvbLayer.GetSetting("ClickfinderDataAvailable").Value = "true" Then

                                    'idProgram in TvMovieProgram suchen & Daten aktualisieren
                                    Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_program.IdProgram)
                                    _TvMovieProgram.idMovingPictures = _MovingPicturesDB(i).MovingPicturesID
                                    _TvMovieProgram.local = True

                                    If Not String.IsNullOrEmpty(_MovingPicturesDB(i).Cover) And String.IsNullOrEmpty(_TvMovieProgram.Cover) Then
                                        _TvMovieProgram.Cover = _MovingPicturesDB(i).Cover
                                    End If

                                    If Not String.IsNullOrEmpty(_MovingPicturesDB(i).FanArt) And String.IsNullOrEmpty(_TvMovieProgram.FanArt) Then
                                        _TvMovieProgram.FanArt = _MovingPicturesDB(i).FanArt
                                    End If

                                    _TvMovieProgram.needsUpdate = True
                                    _TvMovieProgram.Persist()

                                End If

                                'ausgegeben Zahl in log ist höher als import in TvMovieProgram
                                'wegen SQLabfrage nach title & epsiodeName -> Überschneidungen.
                                EPGCounter = EPGCounter + 1

                            Catch ex As Exception
                                MyLog.[Error]("TVMovie: [GetMovingPicturesInfos]: Loop _Result exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                                MyLog.[Error]("TVMovie: [GetMovingPicturesInfos]: title:{0} idchannel:{1} startTime: {2}", _program.Title, _program.ReferencedChannel.DisplayName, _program.StartTime)
                            End Try
                        Next

                    Catch ex As Exception
                        MyLog.[Error]("TVMovie: [GetMovingPicturesInfos]: Loop _MovingPicturesDB - exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                    End Try
                Next

                MyLog.[Info]("TVMovie: [GetMovingPicturesInfos]: Summary: {0} MovingPictures Films found in {1} EPG entries", MovieCounter, EPGCounter)
            Catch ex As Exception
                MyLog.[Error]("TVMovie: [GetMovingPicturesInfos]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            End Try
        End Sub

        Private Sub GetTvMovieHighlights()
            Try
                MyLog.[Debug]("TVMovie: [GetTvMovieHighlights]: start import")
                'Alle Sendungen mit Bewertung laden
                Dim _ClickfinderDB As New ClickfinderDB("SELECT * FROM Sendungen INNER JOIN SendungenDetails ON Sendungen.Pos = SendungenDetails.Pos WHERE Bewertung > 0 ORDER BY SenderKennung ASC", TvMovie.DatabasePath)
                Dim _CounterFound As Integer = 0

                For i As Integer = 0 To _ClickfinderDB.Count - 1
                    Dim _DebugchannelName As String = String.Empty
                    Try
                        'Überprüfen ob SenderKennung gemappt ist
                        Dim sb As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(TvMovieMapping))
                        sb.AddConstraint([Operator].Equals, "stationName", _ClickfinderDB(i).SenderKennung)
                        Dim stmt As SqlStatement = sb.GetStatement(True)
                        Dim _Result As IList(Of TvMovieMapping) = ObjectFactory.GetCollection(GetType(TvMovieMapping), stmt.Execute())

                        'Falls channel gemappt, Sendung im Program suchen
                        If _Result.Count > 0 Then
                            For d As Integer = 0 To _Result.Count - 1

                                Dim _channel As Channel = Channel.Retrieve(_Result(d).IdChannel)
                                _DebugchannelName = _channel.DisplayName

                                Dim sb2 As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(Program))
                                sb2.AddConstraint([Operator].Equals, "startTime", _ClickfinderDB(i).Beginn)
                                sb2.AddConstraint([Operator].Equals, "endTime", _ClickfinderDB(i).Ende)
                                sb2.AddConstraint([Operator].Equals, "idChannel", _channel.IdChannel)
                                Dim stmt2 As SqlStatement = sb2.GetStatement(True)
                                Dim _Program As IList(Of Program) = ObjectFactory.GetCollection(GetType(Program), stmt2.Execute())

                                'Wenn Program gefunden dann in TvMovieProgram schreiben

                                If _Program.Count >= 1 Then

                                    For y As Integer = 0 To _Program.Count - 1
                                        'idProgram in TvMovieProgram suchen & Daten aktualisieren
                                        Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_Program(y).IdProgram)


                                        'nur Informationen die zwigend benötigt werden, anzeige in GuiItems, GuiCategories & GuiHighlights
                                        '+ zusätzlich Infos zum sortieren & suchen (z.B. TvMovieBewretung, Fun, Action, etc.)

                                        'BildDateiname aus Clickfinder DB holen, sofern vorhanden
                                        If CBool(_ClickfinderDB(i).KzBilddateiHeruntergeladen) = True And Not String.IsNullOrEmpty(_ClickfinderDB(i).Bilddateiname) Then
                                            _TvMovieProgram.BildDateiname = _ClickfinderDB(i).Bilddateiname
                                        End If

                                        'TvMovie Bewertung aus Clickfinder DB holen, sofern vorhanden
                                        If Not _ClickfinderDB(i).Bewertung = 0 Then
                                            _TvMovieProgram.TVMovieBewertung = _ClickfinderDB(i).Bewertung
                                        End If

                                        'KurzKritik aus Clickfinder DB holen, sofern vorhanden
                                        If Not String.IsNullOrEmpty(_ClickfinderDB(i).Kurzkritik) Then
                                            _TvMovieProgram.KurzKritik = _ClickfinderDB(i).Kurzkritik
                                        End If

                                        'Bewertungen String aus Clickfinder DB holen, zerlegen, einzel Bewertungen extrahieren
                                        If Not String.IsNullOrEmpty(_ClickfinderDB(i).Bewertungen) Then
                                            ' We want to split this input string
                                            Dim s As String = _ClickfinderDB(i).Bewertungen

                                            ' Split string based on spaces
                                            Dim words As String() = s.Split(New Char() {";"c})

                                            ' Use For Each loop over words and display them
                                            Dim word As String
                                            For Each word In words

                                                'MsgBox(Left(word, InStr(word, "=") - 1))

                                                'MsgBox(CInt(Right(word, word.Length - InStr(word, "="))))

                                                Select Case Left(word, InStr(word, "=") - 1)
                                                    Case Is = "Spaß"
                                                        _TvMovieProgram.Fun = CInt(Right(word, word.Length - InStr(word, "=")))
                                                    Case Is = "Action"
                                                        _TvMovieProgram.Action = CInt(Right(word, word.Length - InStr(word, "=")))
                                                    Case Is = "Erotik"
                                                        _TvMovieProgram.Erotic = CInt(Right(word, word.Length - InStr(word, "=")))
                                                    Case Is = "Spannung"
                                                        _TvMovieProgram.Tension = CInt(Right(word, word.Length - InStr(word, "=")))
                                                    Case Is = "Anspruch"
                                                        _TvMovieProgram.Requirement = CInt(Right(word, word.Length - InStr(word, "=")))
                                                    Case Is = "Gefühl"
                                                        _TvMovieProgram.Feelings = CInt(Right(word, word.Length - InStr(word, "=")))
                                                End Select

                                            Next
                                        End If

                                        'Actors aus Clickfinder DB holen, sofern vorhanden
                                        If Not String.IsNullOrEmpty(_ClickfinderDB(i).Darsteller) Then
                                            _TvMovieProgram.Actors = _ClickfinderDB(i).Darsteller
                                        End If

                                        _TvMovieProgram.needsUpdate = True
                                        _TvMovieProgram.Persist()

                                        _CounterFound = _CounterFound + 1
                                    Next

                                    If _Program.Count > 1 Then
                                        MyLog.Info("Program found in {0} EPG Entries (Start: {1}, Ende: {2}, Titel: {3}, Channel: {4}", _
                                                _Program.Count, _ClickfinderDB(i).Beginn, _ClickfinderDB(i).Ende, _ClickfinderDB(i).Titel, _channel.DisplayName)
                                    End If
                                    
                                Else
                                    'Nur alte Sendungen < 2 Tage sind nicht im EPG enthalten
                                    'Mylog.[Debug]("Start: {0}, Ende: {1}, Titel: {2}, Channel: {3}", _ClickfinderDB(i).Beginn, _ClickfinderDB(i).Ende, _ClickfinderDB(i).Titel, _channel.DisplayName)
                                End If

                            Next
                        End If

                    Catch ex As Exception
                        MyLog.[Error]("TVMovie: [GetTvMovieHighlights]: databasePath: {0} exception err:{1} stack:{2}", TvMovie.DatabasePath, ex.Message, ex.StackTrace)
                        MyLog.[Error]("TVMovie: [GetTvMovieHighlights]: Titel: {0}, SenderKennung:{1}, DisplayName:{2}, Beginn:{3}", _ClickfinderDB(i).Titel, _ClickfinderDB(i).SenderKennung, _DebugchannelName, _ClickfinderDB(i).Beginn)
                    End Try
                Next

                MyLog.[Info]("TVMovie: [GetTvMovieHighlights]: Summary: Infos for {0}/{1} saved in TvMovieProgram", _CounterFound, _ClickfinderDB.Count)

            Catch ex As Exception
                MyLog.[Error]("TVMovie: [GetTvMovieHighlights]: databasePath: {0} exception err:{1} stack:{2}", TvMovie.DatabasePath, ex.Message, ex.StackTrace)
            End Try

        End Sub

        Private Sub GetVideoDatabaseInfos()

            Try
                MyLog.[Debug]("TVMovie: [GetVideoDatabaseInfos]: start import")

                Dim _VideoDB As New VideoDB
                _VideoDB.LoadAllVideoDBFilms()

                Dim EPGCounter As Integer = 0
                Dim MovieCounter As Integer = 0
                Dim _SQLString As String = String.Empty

                For i As Integer = 0 To _VideoDB.Count - 1

                    Try
                        Dim _Result As New ArrayList

                        'nach Mov.Pic Titel suchen
                        _SQLString = _
                            "Select idProgram from program WHERE title LIKE '" & allowedSigns(_VideoDB(i).Title) & "' "

                        'nach Mov.Pic Titel über Dateiname suchen
                        If Not String.IsNullOrEmpty(_VideoDB(i).TitlebyFileName) Then
                            _SQLString = _SQLString & _
                            "OR title LIKE '" & allowedSigns(_VideoDB(i).TitlebyFileName) & "' " & _
                            "OR episodeName LIKE '" & allowedSigns(_VideoDB(i).TitlebyFileName) & "' "
                        End If

                        _Result.AddRange(Broker.Execute(_SQLString).TransposeToFieldList("idProgram", False))

                        If _Result.Count > 0 Then
                            MovieCounter = MovieCounter + 1
                        End If

                        For d As Integer = 0 To _Result.Count - 1

                            Dim _program As Program = Program.Retrieve(CInt(_Result.Item(d)))

                            Try
                                'Daten im EPG (program) updaten

                                'Wenn TvMovieImportMovingPicturesInfos deaktiviert, dann Rating aus VideoDatabase nehmen
                                If _tvbLayer.GetSetting("TvMovieImportMovingPicturesInfos").Value = "false" Then
                                    _program.StarRating = _VideoDB(i).Rating
                                End If

                                If InStr(_program.Description, "existiert lokal") = 0 And String.IsNullOrEmpty(_program.SeriesNum) Then
                                    _program.Description = "existiert lokal" & vbNewLine & _program.Description
                                End If

                                _program.Persist()

                                'Clickfinder ProgramGuide Infos in TvMovieProgram schreiben, sofern aktiviert
                                If _tvbLayer.GetSetting("ClickfinderDataAvailable").Value = "true" Then

                                    'idProgram in TvMovieProgram suchen & Daten aktualisieren
                                    Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_program.IdProgram)
                                    _TvMovieProgram.idVideo = _VideoDB(i).VideoID
                                    _TvMovieProgram.local = True

                                    _TvMovieProgram.needsUpdate = True
                                    _TvMovieProgram.Persist()

                                End If

                                EPGCounter = EPGCounter + 1

                            Catch ex As Exception
                                MyLog.[Error]("TVMovie: [GetVideoDatabaseInfos]: Loop _Result exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                                MyLog.[Error]("TVMovie: [GetVideoDatabaseInfos]: title:{0} idchannel:{1} startTime: {2}", _program.Title, _program.ReferencedChannel.DisplayName, _program.StartTime)
                            End Try
                        Next

                    Catch ex As Exception
                        MyLog.[Error]("TVMovie: [GetVideoDatabaseInfos]: Loop _VideoDB - exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                    End Try
                Next

                MyLog.[Info]("TVMovie: [GetVideoDatabaseInfos]: Summary: {0} Video Films found in {1} EPG entries", MovieCounter, EPGCounter)
            Catch ex As Exception
                MyLog.[Error]("TVMovie: [GetVideoDatabaseInfos]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            End Try

        End Sub

        Private Sub RunApplicationAfterImport()
            If File.Exists(_tvbLayer.GetSetting("TvMovieRunAppAfter").Value) Then

                Dim StartTime As Date = Date.Now

                Dim App As New Process()
                App.StartInfo.FileName = _tvbLayer.GetSetting("TvMovieRunAppAfter").Value

                If _tvbLayer.GetSetting("TvMovieRunAppHidden", "true").Value = "true" Then
                    App.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                Else
                    App.StartInfo.WindowStyle = ProcessWindowStyle.Normal
                End If

                App.Start()
                MyLog.Debug("TVMovie: Application {0} started", _tvbLayer.GetSetting("TvMovieRunAppAfter").Value)

                If _tvbLayer.GetSetting("TvMovieIsEpisodenScanner", "false").Value = "true" Then

                    MyLog.Debug("TVMovie: Application: Wait for exit")
                    App.WaitForExit()
                    MyLog.Debug("TVMovie: EpisodenScanner runtime: {0}", (Date.Now - StartTime).Minutes & "min " & (Date.Now - StartTime).Seconds & "s")
                    CheckEpisodenscannerImports()
                    MyLog.Info("TVMovie: Import duration: {0}", (Date.Now - _ImportStartTime).Minutes & "min " & (Date.Now - _ImportStartTime).Seconds & "s")
                End If

            Else
                MyLog.Error("TVMovie: Error - RunApplicationAfter not found")
            End If
        End Sub

        Private Function allowedSigns(ByVal expression As String) As String
            Return Replace(Replace(expression, "'", "''"), ":", "%")
        End Function

        Private Function getTvMovieProgram(ByVal idProgram As Integer) As TVMovieProgram
            Try
                'idProgram in TvMovieProgram suchen & Daten aktualisieren
                Dim _TvMovieProgram As TVMovieProgram = TVMovieProgram.Retrieve(idProgram)
                Return _TvMovieProgram
            Catch ex As Exception
                'idProgram in TvMovieProgram nicht gefunden -> Daten neu anlegen
                Dim _TvMovieProgram As New TVMovieProgram(idProgram)
                Return _TvMovieProgram
            End Try
        End Function

        Private Sub CheckEpisodenscannerImports()

            MyLog.[Info]("TVMovie: [CheckEpisodenscannerImport]: Process start")

            Dim _Counter As Integer = 0
            

            Try
                'Alle Programme mit Series- & EpisodenNummer laden
                Dim sb As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(Program))
                sb.AddConstraint([Operator].GreaterThan, "episodeNum", "")
                Dim stmt As SqlStatement = sb.GetStatement(True)
                Dim _Result As IList(Of Program) = ObjectFactory.GetCollection(GetType(Program), stmt.Execute())

                If _Result.Count > 0 Then

                    'gefunden durchlaufen
                    For i As Integer = 0 To _Result.Count - 1

                        Try
                            Dim _logSeriesIsLinked As String = String.Empty
                            Dim _SeriesIsLinked As Boolean = False


                            'Prüfen ob die Serie evtl. verlinkt ist.
                            Dim sbSeries As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(TvMovieSeriesMapping))
                            sbSeries.AddConstraint([Operator].Equals, "EpgTitle", _Result(i).Title)
                            Dim stmtSeries As SqlStatement = sbSeries.GetStatement(True)
                            Dim _TvMovieSeriesMapping As IList(Of TvMovieSeriesMapping) = ObjectFactory.GetCollection(GetType(TvMovieSeriesMapping), stmtSeries.Execute())

                            Dim _SeriesName As String = String.Empty

                            'Serie ist verlinkt -> org. SerienName anstatt EPG Name verwenden
                            If _TvMovieSeriesMapping.Count > 0 Then

                                Dim _TvSeriesName As New TVSeriesDB

                                _TvSeriesName.LoadSeriesName(_TvMovieSeriesMapping(0).idSeries)
                                _SeriesName = _TvSeriesName(0).SeriesName

                                _SeriesIsLinked = True
                                _logSeriesIsLinked = "TVMovie: [CheckEpisodenscannerImports]: manuel mapping found: " & _TvSeriesName(0).SeriesName & " -> " & _TvMovieSeriesMapping(0).EpgTitle

                                _TvSeriesName.Dispose()

                            Else
                                'Nicht verlinkt -> EPG Name verwenden
                                _SeriesName = _Result(i).Title
                            End If

                            Dim _TvSeriesDB As New TVSeriesDB
                            _TvSeriesDB.LoadEpisode(allowedSigns(_SeriesName), CInt(_Result(i).SeriesNum), CInt(_Result(i).EpisodeNum))

                            'Serie in TvSeries gefunden
                            If _TvSeriesDB.CountSeries > 0 Then

                                'Falls Episode nicht lokal verfügbar -> im EPG Describtion kennzeichnen
                                If _TvSeriesDB.EpisodeExistLocal = False Then
                                    If InStr(_Result(i).Description, "Neue Folge: " & _Result(i).EpisodeName) = 0 Then
                                        _Result(i).Description = Replace(_Result(i).Description, "Folge: " & _Result(i).EpisodeName, "Neue Folge: " & _Result(i).EpisodeName)
                                        _Result(i).Persist()
                                        If _SeriesIsLinked = True Then
                                            MyLog.[Info](_logSeriesIsLinked)
                                        End If
                                        MyLog.[Info]("TVMovie: [CheckEpisodenscannerImports]: New Episode: {0} - S{1}E{2}", _Result(i).Title, _Result(i).SeriesNum, _Result(i).EpisodeNum)
                                    End If
                                End If
                                _Counter = _Counter + 1


                                If _tvbLayer.GetSetting("ClickfinderDataAvailable", "false").Value = "true" Then

                                    'Pürfen ob in TvMovieProgram existiert
                                    Dim sb2 As New SqlBuilder(Gentle.Framework.StatementType.Select, GetType(TVMovieProgram))
                                    sb2.AddConstraint([Operator].Equals, "idprogram", _Result(i).IdProgram)
                                    Dim stmt2 As SqlStatement = sb2.GetStatement(True)
                                    Dim _SeriesIsInTvMovieProgram As IList(Of TVMovieProgram) = ObjectFactory.GetCollection(GetType(TVMovieProgram), stmt2.Execute())

                                    'TvMovieProgram laden / neu anlegen
                                    Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_Result(i).IdProgram)

                                    'Sofern idSeries = 0 -> Daten updaten & speichern
                                    If _TvMovieProgram.idSeries = 0 Then

                                        If _SeriesIsInTvMovieProgram.Count = 0 Then
                                            If _SeriesIsLinked = True Then
                                                MyLog.[Info](_logSeriesIsLinked)
                                            End If
                                            MyLog.[Info]("TVMovie: [CheckEpisodenscannerImports]: New entry in TvMovieProgram table")
                                        Else
                                            If _SeriesIsLinked = True Then
                                                MyLog.[Info](_logSeriesIsLinked)
                                            End If
                                            MyLog.[Info]("TVMovie: [CheckEpisodenscannerImports]: Updated entry in TvMovieProgram table")
                                        End If

                                        _TvMovieProgram.idSeries = _TvSeriesDB(0).SeriesID
                                        _TvMovieProgram.idEpisode = _TvSeriesDB.EpisodeCompositeID
                                        _TvMovieProgram.local = True

                                        'Episoden Image
                                        If Not String.IsNullOrEmpty(_TvSeriesDB.EpisodeImage) = True Then
                                            _TvMovieProgram.EpisodeImage = _TvSeriesDB.EpisodeImage
                                        End If

                                        'Serien Poster Image
                                        If Not String.IsNullOrEmpty(_TvSeriesDB(0).SeriesPosterImage) = True Then
                                            _TvMovieProgram.SeriesPosterImage = _TvSeriesDB(0).SeriesPosterImage
                                        End If

                                        'FanArt
                                        If Not String.IsNullOrEmpty(_TvSeriesDB(0).FanArt) = True Then
                                            _TvMovieProgram.FanArt = _TvSeriesDB(0).FanArt
                                        End If

                                        If _TvSeriesDB.EpisodeExistLocal = False Then
                                            _TvMovieProgram.local = False
                                        End If

                                        _TvMovieProgram.needsUpdate = True
                                        _TvMovieProgram.Persist()

                                    End If

                                End If

                            Else
                                'Episode nicht in TvSeries DB gefunden (=neue Aufnahme), dann als neu markieren
                                If InStr(_Result(i).Description, "Neue Folge: " & _Result(i).EpisodeName) = 0 Then
                                    _Result(i).Description = Replace(_Result(i).Description, "Folge: " & _Result(i).EpisodeName, "Neue Folge: " & _Result(i).EpisodeName)
                                    _Result(i).Persist()
                                End If

                                If _tvbLayer.GetSetting("ClickfinderDataAvailable", "false").Value = "true" Then
                                    Dim _TvMovieProgram As TVMovieProgram = getTvMovieProgram(_Result(i).IdProgram)
                                    _TvMovieProgram.idSeries = 1
                                    _TvMovieProgram.local = False
                                    _TvMovieProgram.Persist()
                                End If

                                _Counter = _Counter + 1
                                MyLog.[Debug]("TVMovie: [CheckEpisodenscannerImports]: Episode: {0} - S{1}E{2} not found in MP-TvSeries DB -> mark as Neue Folge", _Result(i).Title, _Result(i).SeriesNum, _Result(i).EpisodeNum)
                            End If

                            _TvSeriesDB.Dispose()

                        Catch ex As Exception
                            MyLog.[Error]("TVMovie: [CheckEpisodenscannerImports]: Loop: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                        End Try
                    Next
                End If

                MyLog.[Info]("TVMovie: [CheckEpisodenscannerImports]: Process success - {0} Series imported", _Counter)

            Catch ex As Exception
                MyLog.[Error]("TVMovie: [CheckEpisodenscannerImports]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            End Try
        End Sub

        Private Function MySqlDate(ByVal Datum As Date) As String
            Return "'" & Datum.Year & "-" & Format(Datum.Month, "00") & "-" & Format(Datum.Day, "00") & " " & Format(Datum.Hour, "00") & ":" & Format(Datum.Minute, "00") & ":00'"
        End Function

#End Region
    End Class

    ' class
End Namespace
