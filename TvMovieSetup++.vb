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
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports System.Windows.Forms
Imports TvDatabase
Imports TvEngine
Imports TvLibrary.Log
Imports SetupTv
Imports TvMovie.TvEngine.TvMovie


Namespace SetupTv.Sections
    Partial Public Class TvMovieSetup
        Inherits SectionSettings

#Region "ChannelInfo class"

        Private Class ChannelInfo
            Private _start As String = "00:00"
            Private _end As String = "00:00"
            Private _name As String = String.Empty

            Public Property Start() As String
                Get
                    Return _start
                End Get
                Set(ByVal value As String)
                    _start = value
                End Set
            End Property

            Public Property [End]() As String
                Get
                    Return _end
                End Get
                Set(ByVal value As String)
                    _end = value
                End Set
            End Property

            Public Property Name() As String
                Get
                    Return _name
                End Get
                Set(ByVal value As String)
                    _name = value
                End Set
            End Property

            Public Sub New()
                _start = "00:00"
                _end = "00:00"
            End Sub
        End Class

#End Region

#Region "Form Methods"

        Private Sub treeViewStations_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles treeViewTvMStations.DoubleClick
            If treeViewTvMStations.SelectedNode IsNot Nothing Then
                treeViewTvMStations.SelectedNode.Collapse()
            End If
            MapStation()
        End Sub

        Private Sub treeViewChannels_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles treeViewMpChannels.DoubleClick
            UnmapStation()
        End Sub

        Private Sub linkLabelInfo_LinkClicked(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs) Handles linkLabelInfo.LinkClicked
            Process.Start("http://www.tvmovie.de/ClickFinder.57.0.html")
        End Sub
        Private Sub LinkClickfinderPG_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkClickfinderPG.LinkClicked
            Process.Start("http://www.team-mediaportal.com/extensions/television/clickfinder-programguide?lang=en")
        End Sub
        Private Sub Linklabel_EpSc_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles Linklabel_EpSc.LinkClicked
            Process.Start("http://forum.team-mediaportal.com/electronic-program-guide-67/new-tool-episodescanner-adds-series-episodenumbers-your-mediaportal-4tr-epg-76220/")
        End Sub
#End Region

#Region "Constructor"

        Public Sub New()
            Me.New("TV Movie Clickfinder EPG import")
        End Sub

        Public Sub New(ByVal name As String)
            MyBase.New(name)
            InitializeComponent()
        End Sub

#End Region

#Region "Serialisation"

        Public Overrides Sub OnSectionDeActivated()
            If tabControlTvMovie.SelectedIndex = 2 Then
                SaveMapping()
            End If

            SaveDbSettings()

            MyBase.OnSectionDeActivated()
        End Sub

        Private Sub SaveDbSettings()
            Dim layer As New TvBusinessLayer()

            DatabasePath = tbDbPath.Text

            Dim setting As Setting = layer.GetSetting("TvMovieEnabled", "false")
            If checkBoxEnableImport.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieShortProgramDesc", "false")
            If checkBoxUseShortDesc.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieExtendDescription", "true")
            If checkBoxAdditionalInfo.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieShowAudioFormat", "false")
            If checkBoxShowAudioFormat.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieSlowImport", "true")
            If checkBoxSlowImport.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieShowRatings", "true")
            If checkBoxShowRatings.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieLimitActors", "5")
            If checkBoxLimitActors.Checked Then
                setting.Value = numericUpDownActorCount.Value.ToString()
            Else
                setting.Value = "0"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieShowLive", "true")
            If checkBoxShowLive.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieShowRepeating", "false")
            If checkBoxShowRepeat.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieRestPeriod", "24")
            setting.Value = GetRestPeriod()
            setting.Persist()

            'TV Movie++ Enhancement by Scrounger
            setting = layer.GetSetting("TvMovieRunAppAfter", String.Empty)
            setting.Value = tbRunAppAfter.Text
            setting.Persist()

            setting = layer.GetSetting("TvMovieRunAppHidden", "true")
            If checkBoxRunHidden.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieIsEpisodenScanner", "false")
            If CheckBoxEpSc.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database")
            setting.Value = tbMPDatabasePath.Text
            setting.Persist()

            setting = layer.GetSetting("TvMovieImportTvSeriesInfos", "false")
            If CheckBoxTvSeries.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieImportMovingPicturesInfos", "false")
            If CheckBoxMovingPictures.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieImportMyFilmsInfos", "false")
            If CheckBoxMyFilms.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("TvMovieImportVideoDatabaseInfos", "false")
            If CheckBoxVideoDB.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

            setting = layer.GetSetting("ClickfinderDataAvailable", "false")
            If CheckBoxClickfinderPG.Checked Then
                setting.Value = "true"
            Else
                setting.Value = "false"
            End If
            setting.Persist()

        End Sub

        Public Overrides Sub OnSectionActivated()
            LoadDbSettings()

            MyBase.OnSectionActivated()
        End Sub

        Private Sub LoadDbSettings()
            Dim layer As New TvBusinessLayer()
            checkBoxEnableImport.Checked = layer.GetSetting("TvMovieEnabled", "false").Value = "true"
            checkBoxUseShortDesc.Checked = layer.GetSetting("TvMovieShortProgramDesc", "false").Value = "true"
            checkBoxAdditionalInfo.Checked = layer.GetSetting("TvMovieExtendDescription", "true").Value = "true"
            checkBoxShowRatings.Checked = layer.GetSetting("TvMovieShowRatings", "true").Value = "true"
            checkBoxShowAudioFormat.Checked = layer.GetSetting("TvMovieShowAudioFormat", "false").Value = "true"
            checkBoxSlowImport.Checked = layer.GetSetting("TvMovieSlowImport", "true").Value = "true"
     

            Dim tvMovieLimitActors As Decimal = Convert.ToDecimal(layer.GetSetting("TvMovieLimitActors", "5").Value)
            If tvMovieLimitActors < numericUpDownActorCount.Minimum OrElse tvMovieLimitActors > numericUpDownActorCount.Maximum Then
                checkBoxLimitActors.Checked = False
                numericUpDownActorCount.Value = numericUpDownActorCount.Minimum
            Else
                checkBoxLimitActors.Checked = True
                numericUpDownActorCount.Value = tvMovieLimitActors
            End If

            checkBoxShowLive.Checked = layer.GetSetting("TvMovieShowLive", "true").Value = "true"
            checkBoxShowRepeat.Checked = layer.GetSetting("TvMovieShowRepeating", "false").Value = "true"
            SetRestPeriod(layer.GetSetting("TvMovieRestPeriod", "24").Value)


            'TV Movie++ Enhancement by Scrounger
            tbRunAppAfter.Text = layer.GetSetting("TvMovieRunAppAfter", String.Empty).Value
            tbMPDatabasePath.Text = layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value
            checkBoxRunHidden.Checked = layer.GetSetting("TvMovieRunAppHidden", "true").Value = "true"
            CheckBoxEpSc.Checked = layer.GetSetting("TvMovieIsEpisodenScanner", "false").Value = "true"
            CheckBoxTvSeries.Checked = layer.GetSetting("TvMovieImportTvSeriesInfos", "false").Value = "true"
            CheckBoxMovingPictures.Checked = layer.GetSetting("TvMovieImportMovingPicturesInfos", "false").Value = "true"
            CheckBoxMyFilms.Checked = layer.GetSetting("TvMovieImportMyFilmsInfos", "false").Value = "true"
            CheckBoxVideoDB.Checked = layer.GetSetting("TvMovieImportVideoDatabaseInfos", "false").Value = "true"
            CheckBoxClickfinderPG.Checked = layer.GetSetting("ClickfinderDataAvailable", "false").Value = "true"

            If CheckBoxTvSeries.Checked = True Then
                ButtonSeriesMapping.Enabled = True
            Else
                ButtonSeriesMapping.Enabled = False
            End If


        End Sub

#End Region

#Region "Mapping methods"

        ''' <summary>
        ''' Load stations from databases and fill controls with that data
        ''' </summary>
        Private Sub LoadStations(ByVal localInstall As Boolean)
            Dim database As New TvEngine.TvMovieDatabase()
            If database.Connect() Then
                Try
                    treeViewTvMStations.BeginUpdate()
                    Try
                        treeViewTvMStations.Nodes.Clear()
                        imageListTvmStations.Images.Clear()
                        treeViewTvMStations.ItemHeight = If(localInstall, 24, 16)

                        Dim GifBasePath As String = TVMovieProgramPath & "Gifs\"


                        For i As Integer = 0 To database.Stations.Count - 1
                            Try
                                Dim station As TvEngine.TVMChannel = database.Stations(i)

                                If localInstall Then
                                    Dim channelLogo As String = GifBasePath & station.TvmZeichen
                                    If Not File.Exists(channelLogo) Then
                                        channelLogo = GifBasePath & "tvmovie_senderlogoplatzhalter.gif"
                                    End If

                                    ' convert gif to ico
                                    Dim tvmLogo As New Bitmap(channelLogo)
                                    Dim iconHandle As IntPtr = tvmLogo.GetHicon()
                                    Dim stationThumb As Icon = Icon.FromHandle(iconHandle)
                                    imageListTvmStations.Images.Add(New Icon(stationThumb, New Size(32, 22)))
                                End If

                                Dim stationNode As New TreeNode(station.TvmEpgDescription, i, i)
                                ', subItems);
                                Dim channelInfo As New ChannelInfo()
                                channelInfo.Name = station.TvmEpgChannel
                                stationNode.Tag = channelInfo
                                treeViewTvMStations.Nodes.Add(stationNode)
                            Catch exstat As Exception
                                MyLog.Info("TvMovieSetup: Error loading TV Movie station - {0}", exstat.Message)
                            End Try
                        Next
                    Finally
                        treeViewTvMStations.EndUpdate()
                    End Try

                    treeViewMpChannels.BeginUpdate()
                    Try
                        treeViewMpChannels.Nodes.Clear()
                        Dim mpChannelList As List(Of Channel) = database.GetChannels()
                        For Each channel As Channel In mpChannelList
                            'TreeNode[] subItems = new TreeNode[] { new TreeNode(channel.IdChannel.ToString()), new TreeNode(channel.DisplayName) };
                            Dim stationNode As New TreeNode(channel.DisplayName)
                            stationNode.Tag = channel
                            treeViewMpChannels.Nodes.Add(stationNode)
                        Next
                    Catch exdb As Exception
                        MyLog.Info("TvMovieSetup: Error loading MP's channels from database - {0}", exdb.Message)
                    Finally
                        treeViewMpChannels.EndUpdate()
                    End Try
                Catch ex As Exception
                    MyLog.Info("TvMovieSetup: Unhandled error in  LoadStations - {0}" & vbLf & "{1}", ex.Message, ex.StackTrace)
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Map selected TVMovie station to a selected MP channel
        ''' </summary>
        Private Sub MapStation()
            Dim selectedChannel As TreeNode = treeViewMpChannels.SelectedNode
            If selectedChannel Is Nothing Then
                Return
            End If
            While selectedChannel.Parent IsNot Nothing
                selectedChannel = selectedChannel.Parent
            End While

            Dim selectedStation As TreeNode = DirectCast(treeViewTvMStations.SelectedNode.Clone(), TreeNode)

            For Each stationNode As TreeNode In selectedChannel.Nodes
                If stationNode.Text = selectedStation.Text Then
                    Return
                End If
            Next

            If selectedChannel.Nodes.Count > 0 Then
                selectedChannel.Nodes(0).ForeColor = Color.Green
                selectedStation.ForeColor = Color.Green
            Else
                selectedStation.ForeColor = Color.Blue
            End If

            selectedChannel.Nodes.Add(selectedStation)
            selectedChannel.Expand()
        End Sub

        ''' <summary>
        ''' Remove TVMovie station mapping from selected MP channel
        ''' </summary>
        Private Sub UnmapStation()
            Dim selectedChannel As TreeNode = treeViewMpChannels.SelectedNode
            If selectedChannel Is Nothing Then
                Return
            End If
            If selectedChannel.Parent IsNot Nothing Then
                If selectedChannel.Parent.Nodes.Count = 2 Then
                    selectedChannel.Parent.Nodes(0).ForeColor = Color.Blue
                    selectedChannel.Parent.Nodes(1).ForeColor = Color.Blue
                End If
                selectedChannel.Remove()
            Else
                selectedChannel.Nodes.Clear()
            End If
        End Sub

        ''' <summary>
        ''' Save station-channel mapping to database
        ''' </summary>
        Private Sub SaveMapping()
            Dim mappingList As IList(Of TvMovieMapping) = TvMovieMapping.ListAll()

            If mappingList IsNot Nothing AndAlso mappingList.Count > 0 Then
                For Each mapping As TvMovieMapping In mappingList
                    mapping.Remove()
                Next
            Else
                MyLog.Info("TvMovieSetup: SaveMapping - no mappingList items")
            End If

            Dim layer As New TvBusinessLayer()

            For Each channel As TreeNode In treeViewMpChannels.Nodes
                'Mylog.Debug("TvMovieSetup: Processing channel {0}", channel.Text);
                For Each station As TreeNode In channel.Nodes
                    Dim channelInfo As ChannelInfo = DirectCast(station.Tag, ChannelInfo)
                    'Mylog.Debug("TvMovieSetup: Processing channelInfo {0}", channelInfo.Name);
                    Dim mapping As TvMovieMapping = Nothing
                    Try
                        mapping = New TvMovieMapping(DirectCast(channel.Tag, Channel).IdChannel, channelInfo.Name, channelInfo.Start, channelInfo.[End])
                    Catch exm As Exception
                        MyLog.[Error]("TvMovieSetup: Error on new TvMovieMapping for channel {0} - {1}", channel.Text, exm.Message)
                    End Try
                    'Mylog.Write("TvMovieSetup: SaveMapping - new mapping for {0}/{1}", channel.Text, channelInfo.Name);
                    Try
                        MyLog.Debug("TvMovieSetup: Persisting TvMovieMapping for channel {0}", channel.Text)
                        mapping.Persist()
                    Catch ex As Exception
                        MyLog.[Error]("TvMovieSetup: Error on mapping.Persist() {0},{1}", ex.Message, ex.StackTrace)
                    End Try
                Next
            Next
        End Sub

        ''' <summary>
        ''' Load station-channel mapping from database
        ''' </summary>
        Private Sub LoadMapping()
            treeViewMpChannels.BeginUpdate()
            Try
                For Each treeNode As TreeNode In treeViewMpChannels.Nodes
                    For Each childNode As TreeNode In treeNode.Nodes
                        childNode.Remove()
                    Next
                Next
                Try
                    Dim mappingDb As IList(Of TvMovieMapping) = TvMovieMapping.ListAll()
                    If mappingDb IsNot Nothing AndAlso mappingDb.Count > 0 Then
                        For Each mapping As TvMovieMapping In mappingDb
                            Dim MpChannelName As String = String.Empty
                            Try
                                Dim channelNode As TreeNode = FindChannel(mapping.IdChannel)
                                If channelNode IsNot Nothing Then
                                    Dim stationName As String = mapping.StationName
                                    If FindStation(stationName) IsNot Nothing Then
                                        Dim stationNode As TreeNode = DirectCast(FindStation(stationName).Clone(), TreeNode)
                                        Dim channelInfo As New ChannelInfo()
                                        If stationNode IsNot Nothing Then
                                            Dim start As String = mapping.TimeSharingStart
                                            Dim [end] As String = mapping.TimeSharingEnd

                                            If start <> "00:00" OrElse [end] <> "00:00" Then
                                                stationNode.Text = String.Format("{0} ({1}-{2})", stationName, start, [end])
                                            Else
                                                stationNode.Text = String.Format("{0}", stationName)
                                            End If

                                            channelInfo.Start = start
                                            channelInfo.[End] = [end]
                                            channelInfo.Name = stationName

                                            stationNode.Tag = channelInfo

                                            channelNode.Nodes.Add(stationNode)
                                            channelNode.Expand()
                                        End If
                                    Else
                                        MyLog.Debug("TVMovie plugin: Channel {0} no longer present in Database - ignoring", stationName)
                                    End If
                                End If
                            Catch exInner As Exception
                                MyLog.Debug("TVMovie plugin: Mapping of station {0} failed; maybe it has been deleted / changed ({1})", MpChannelName, exInner.Message)
                            End Try
                        Next
                    Else
                        MyLog.Debug("TVMovie plugin: LoadMapping did not find any mapped channels")
                    End If
                Catch ex As Exception
                    MyLog.Debug("TVMovie plugin: LoadMapping failed - {0},{1}", ex.Message, ex.StackTrace)
                End Try
                ColorTree()
            Finally
                treeViewMpChannels.EndUpdate()
            End Try
        End Sub

        Private Function FindChannel(ByVal mpChannelId As Integer) As TreeNode
            For Each MpNode As TreeNode In treeViewMpChannels.Nodes
                If MpNode.Tag IsNot Nothing Then
                    Dim checkChannel As Channel = TryCast(MpNode.Tag, Channel)
                    If checkChannel IsNot Nothing Then
                        If checkChannel.IdChannel = mpChannelId Then
                            Return MpNode
                        End If
                    Else
                        MyLog.Debug("TVMovie plugin: FindChannel failed - no Channel in Node tag of {0}", MpNode.Text)
                    End If
                End If
            Next
            Return Nothing
        End Function

        Private Function FindStation(ByVal aTvMStationName As String) As TreeNode
            For Each TvMNode As TreeNode In treeViewTvMStations.Nodes
                If TvMNode.Tag IsNot Nothing Then
                    If DirectCast(TvMNode.Tag, ChannelInfo).Name = aTvMStationName Then
                        Return TvMNode
                    End If
                End If
            Next

            Return Nothing
        End Function

        Private Sub ColorTree()
            For Each parentNode As TreeNode In treeViewMpChannels.Nodes
                For Each subNode As TreeNode In parentNode.Nodes
                    If parentNode.Nodes.Count > 1 Then
                        subNode.ForeColor = Color.Green
                    Else
                        subNode.ForeColor = Color.Blue
                    End If
                Next
            Next
        End Sub

        Private Sub ColorNode(ByVal channelNode As TreeNode, ByVal color As Color)
            For Each stationNode As TreeNode In channelNode.Nodes
                stationNode.ForeColor = color
            Next
        End Sub

        Private Sub treeViewChannels_AfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles treeViewMpChannels.AfterSelect
            If e.Node.Parent Is Nothing OrElse e.Node.Tag Is Nothing Then
                panelTimeSpan.Visible = False
                Return
            End If
            panelTimeSpan.Visible = True
            Dim channelInfo As ChannelInfo = DirectCast(e.Node.Tag, ChannelInfo)
            maskedTextBoxTimeStart.Text = channelInfo.Start
            maskedTextBoxTimeEnd.Text = channelInfo.[End]
        End Sub

        Private Function CleanInput(ByVal input As String) As String
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

            Return String.Format("{0:00}:{1:00}", hours, minutes)
        End Function

        Private Sub maskedTextBoxTimeStart_Validated(ByVal sender As Object, ByVal e As EventArgs) Handles maskedTextBoxTimeStart.Validated
            Dim channelInfo As ChannelInfo = DirectCast(treeViewMpChannels.SelectedNode.Tag, ChannelInfo)
            channelInfo.Start = CleanInput(maskedTextBoxTimeStart.Text)
            maskedTextBoxTimeStart.Text = CleanInput(maskedTextBoxTimeStart.Text)
            treeViewMpChannels.SelectedNode.Tag = channelInfo
            If channelInfo.Start <> "00:00" OrElse channelInfo.[End] <> "00:00" Then
                treeViewMpChannels.SelectedNode.Text = String.Format("{0} ({1}-{2})", channelInfo.Name, channelInfo.Start, channelInfo.[End])
            Else
                treeViewMpChannels.SelectedNode.Text = String.Format("{0}", channelInfo.Name)
            End If
        End Sub

        Private Sub maskedTextBoxTimeEnd_Validated(ByVal sender As Object, ByVal e As EventArgs) Handles maskedTextBoxTimeEnd.Validated
            Dim channelInfo As ChannelInfo = DirectCast(treeViewMpChannels.SelectedNode.Tag, ChannelInfo)
            channelInfo.[End] = CleanInput(maskedTextBoxTimeEnd.Text)
            maskedTextBoxTimeEnd.Text = CleanInput(maskedTextBoxTimeEnd.Text)
            treeViewMpChannels.SelectedNode.Tag = channelInfo
            If channelInfo.Start <> "00:00" OrElse channelInfo.[End] <> "00:00" Then
                treeViewMpChannels.SelectedNode.Text = String.Format("{0} ({1}-{2})", channelInfo.Name, channelInfo.Start, channelInfo.[End])
            Else
                treeViewMpChannels.SelectedNode.Text = String.Format("{0}", channelInfo.Name)
            End If
        End Sub

#End Region

#Region "Form settings"

        Private Sub checkBoxUseShortDesc_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If checkBoxUseShortDesc.Checked Then
                checkBoxAdditionalInfo.Checked = False
                checkBoxAdditionalInfo.Enabled = False
            End If
            checkBoxAdditionalInfo.Enabled = True
        End Sub

        Private Sub checkBoxAdditionalInfo_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
            If checkBoxAdditionalInfo.Checked Then
                checkBoxUseShortDesc.Checked = False
            End If
        End Sub

        Private Function GetRestPeriod() As String
            If radioButton6h.Checked Then
                Return "6"
            ElseIf radioButton12h.Checked Then
                Return "12"
            ElseIf radioButton24h.Checked Then
                Return "24"
            ElseIf radioButton2d.Checked Then
                Return "48"
            ElseIf radioButton7d.Checked Then
                Return "168"
            End If

            Return "24"
        End Function

        Private Sub SetRestPeriod(ByVal RadioButtonSetting As String)
            Select Case RadioButtonSetting
                Case "6"
                    radioButton6h.Checked = True
                    Exit Select
                Case "12"
                    radioButton12h.Checked = True
                    Exit Select
                Case "24"
                    radioButton24h.Checked = True
                    Exit Select
                Case "48"
                    radioButton2d.Checked = True
                    Exit Select
                Case "168"
                    radioButton7d.Checked = True
                    Exit Select
                Case Else
                    radioButton24h.Checked = True
                    Exit Select
            End Select
        End Sub

        Private Sub tabControlTvMovie_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles tabControlTvMovie.SelectedIndexChanged
            If Not checkBoxEnableImport.Checked Then
                tabControlTvMovie.SelectedIndex = 0
            ElseIf tabControlTvMovie.SelectedIndex = 0 Then
                SaveMapping()
            End If
        End Sub

        Private Sub checkBoxEnableImport_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkBoxEnableImport.CheckedChanged
            groupBoxDescriptions.Enabled = InlineAssignHelper(groupBoxImportTime.Enabled, InlineAssignHelper(groupBoxInstallMethod.Enabled, checkBoxEnableImport.Checked))

            If checkBoxEnableImport.Checked Then
                rbLocal.Enabled = InlineAssignHelper(rbLocal.Checked, Not String.IsNullOrEmpty(TVMovieProgramPath))
                rbManual.Checked = Not rbLocal.Checked
                tbDbPath.Text = DatabasePath

                Try
                    LoadStations(rbLocal.Checked)
                Catch ex1 As Exception
                    MessageBox.Show(Me, "Please make sure a supported TV Movie Clickfinder release has been successfully installed.", "Error loading TV Movie stations", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                    checkBoxEnableImport.Checked = False
                    MyLog.Info("TVMovie plugin: Error enabling TV Movie import in LoadStations() - {0},{1}", ex1.Message, ex1.StackTrace)
                    Return
                End Try

                Try
                    LoadMapping()
                Catch ex2 As Exception
                    MessageBox.Show(Me, "Please make sure your using a valid channel mapping.", "Error loading TVM <-> MP channel mapping", MessageBoxButtons.OK, MessageBoxIcon.[Error])
                    checkBoxEnableImport.Checked = False
                    MyLog.Info("TVMovie plugin: Error enabling TV Movie import in LoadMapping() - {0},{1}", ex2.Message, ex2.StackTrace)
                    Return
                End Try
            Else
                SaveMapping()
            End If
        End Sub

#End Region

#Region "Manual import methods"

        ''' <summary>
        ''' Inmediately updates and imports EPG data
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub buttonImportNow_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonImportNow.Click
            buttonImportNow.Enabled = False
            SaveDbSettings()
            Try
                Dim manualThread As New Thread(New ThreadStart(AddressOf ManualImportThread))
                manualThread.Name = "TV Movie manual importer"
                manualThread.Priority = ThreadPriority.Normal
                manualThread.IsBackground = False
                manualThread.Start()
            Catch ex2 As Exception
                MyLog.[Error]("TVMovie: Error spawing import thread - {0},{1}", ex2.Message, ex2.StackTrace)
                buttonImportNow.Enabled = True
            End Try
        End Sub

        Private Sub ManualImportThread()
            Dim _database As New TvEngine.TvMovieDatabase()
            Try
                _database.LaunchTVMUpdater(False)
                AddHandler _database.OnStationsChanged, New TvEngine.TvMovieDatabase.StationsChanged(AddressOf _database_OnStationsChanged)
                If _database.Connect() Then
                    _database.Import()
                End If
                buttonImportNow.Enabled = True
            Catch ex As Exception
                MyLog.Info("TvMovie plugin error:")
                MyLog.Write(ex)
                buttonImportNow.Enabled = True
            End Try
        End Sub

        Private Sub _database_OnStationsChanged(ByVal value As Integer, ByVal maximum As Integer, ByVal text As String)
            progressBarImportTotal.Maximum = maximum
            If value <= maximum AndAlso value >= 0 Then
                progressBarImportTotal.Value = value
            End If
        End Sub

        Private Sub rbLocal_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rbLocal.CheckedChanged
            tbDbPath.Enabled = Not rbLocal.Checked
        End Sub

        Private Sub buttonBrowse_Click(ByVal sender As Object, ByVal e As EventArgs) Handles buttonBrowse.Click
            fileDialogDb.Filter = "Access database (*.mdb)|*.mdb|All files (*.*)|*.*"
            fileDialogDb.InitialDirectory = tbDbPath.Text
            If fileDialogDb.ShowDialog(Me) = DialogResult.OK Then
                DatabasePath = InlineAssignHelper(tbDbPath.Text, fileDialogDb.FileName)
                checkBoxEnableImport_CheckedChanged(sender, Nothing)
            End If
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
            target = value
            Return value
        End Function

#End Region

#Region "TVMovie++ enhancement"
        Private Sub BTAppBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTAppBrowse.Click
            Dim openFileDialog1 As New OpenFileDialog()

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            openFileDialog1.Filter = "Application (*.exe)|*.exe|All files (*.*)|*.*"

            If openFileDialog1.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
                tbRunAppAfter.Text = openFileDialog1.FileName
            End If

        End Sub

        Private Sub ButtonBrowseMPDatabases_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonBrowseMPDatabases.Click
            ' First create a FolderBrowserDialog object
            Dim FolderBrowserDialog1 As New FolderBrowserDialog

            ' Then use the following code to create the Dialog window
            ' Change the .SelectedPath property to the default location
            With FolderBrowserDialog1
                ' Desktop is the root folder in the diaMylog.
                .RootFolder = Environment.SpecialFolder.Desktop
                ' Select the C:\Windows directory on entry.
                .SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
                ' Prompt the user with a custom message.
                .Description = "Select the source directory"
                If .ShowDialog = DialogResult.OK Then
                    ' Display the selected folder if the user clicked on the OK button.
                    tbMPDatabasePath.Text = .SelectedPath
                End If
            End With
        End Sub
        Private Sub ButtonEPGgrab_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonEPGgrab.Click
            SaveMapping()

            Dim EPGgrab As New frmEPGgrab
            EPGgrab.ShowDialog()
        End Sub
#End Region

        Private Sub CheckBoxTvSeries_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxTvSeries.CheckedChanged
            Dim layer As New TvBusinessLayer()

            If CheckBoxTvSeries.Checked Then
                ButtonSeriesMapping.Enabled = True
            Else
                ButtonSeriesMapping.Enabled = False
            End If

        End Sub

        Private Sub ButtonSeriesMapping_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonSeriesMapping.Click

            If File.Exists(tbMPDatabasePath.Text & "\" & "TVSeriesDatabase4.db3") Then
                SaveMapping()

                Dim SeriesMapping As New frmSeriesMapping
                SeriesMapping.ShowDialog()
            Else
                MsgBox("TvSeries Datenbank nicht gefunden !", MsgBoxStyle.Critical, "Fehler")
            End If

        End Sub
    End Class
End Namespace
