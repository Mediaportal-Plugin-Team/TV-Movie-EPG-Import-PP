#Region "Copyright (C) 2006-2009 Team MediaPortal"

' 
' *	Copyright (C) 2006-2009 Team MediaPortal
' *	http://www.team-mediaportal.com
' *
' *  This Program is free software; you can redistribute it and/or modify
' *  it under the terms of the GNU General Public License as published by
' *  the Free Software Foundation; either version 2, or (at your option)
' *  any later version.
' *   
' *  This Program is distributed in the hope that it will be useful,
' *  but WITHOUT ANY WARRANTY; without even the implied warranty of
' *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
' *  GNU General Public License for more details.
' *   
' *  You should have received a copy of the GNU General Public License
' *  along with GNU Make; see the file COPYING.  If not, write to
' *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
' *  http://www.gnu.org/copyleft/gpl.html
' *
' 


#End Region

Namespace SetupTv.Sections
    <CLSCompliant(False)> _
    Partial Class TvMovieSetup
        ''' <summary> 
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary> 
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Component Designer generated code"

        ''' <summary> 
        ''' Required method for Designer support - do not modify 
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TvMovieSetup))
            Me.openFileDialog = New System.Windows.Forms.OpenFileDialog
            Me.tabControlTvMovie = New MediaPortal.UserInterface.Controls.MPTabControl
            Me.tabPageSettings = New System.Windows.Forms.TabPage
            Me.linkLabelInfo = New System.Windows.Forms.LinkLabel
            Me.groupBoxEnableTvMovie = New MediaPortal.UserInterface.Controls.MPGroupBox
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.Linklabel_EpSc = New System.Windows.Forms.LinkLabel
            Me.CheckBoxEpSc = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.checkBoxRunHidden = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.tbRunAppAfter = New System.Windows.Forms.TextBox
            Me.Label2 = New System.Windows.Forms.Label
            Me.BTAppBrowse = New System.Windows.Forms.Button
            Me.groupBoxInstallMethod = New System.Windows.Forms.GroupBox
            Me.buttonBrowse = New System.Windows.Forms.Button
            Me.lbDbPath = New System.Windows.Forms.Label
            Me.rbManual = New System.Windows.Forms.RadioButton
            Me.rbLocal = New System.Windows.Forms.RadioButton
            Me.tbDbPath = New System.Windows.Forms.TextBox
            Me.groupBoxImportTime = New System.Windows.Forms.GroupBox
            Me.tbImportStartTime = New System.Windows.Forms.MaskedTextBox
            Me.MpCheckBoxStartImportAtTime = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.progressBarImportTotal = New System.Windows.Forms.ProgressBar
            Me.buttonImportNow = New System.Windows.Forms.Button
            Me.checkBoxSlowImport = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.radioButton7d = New System.Windows.Forms.RadioButton
            Me.radioButton2d = New System.Windows.Forms.RadioButton
            Me.radioButton24h = New System.Windows.Forms.RadioButton
            Me.radioButton12h = New System.Windows.Forms.RadioButton
            Me.radioButton6h = New System.Windows.Forms.RadioButton
            Me.checkBoxEnableImport = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.tabPageImportOptions = New System.Windows.Forms.TabPage
            Me.GroupBox2 = New System.Windows.Forms.GroupBox
            Me.CheckBoxTheTvDb = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.ButtonSeriesMapping = New System.Windows.Forms.Button
            Me.CheckBoxVideoDB = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.CheckBoxMyFilms = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.CheckBoxMovingPictures = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.CheckBoxTvSeries = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.ButtonBrowseMPDatabases = New System.Windows.Forms.Button
            Me.Label3 = New System.Windows.Forms.Label
            Me.tbMPDatabasePath = New System.Windows.Forms.TextBox
            Me.groupBoxDescriptions = New MediaPortal.UserInterface.Controls.MPGroupBox
            Me.checkBoxShowRepeat = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.checkBoxShowLive = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.numericUpDownActorCount = New System.Windows.Forms.NumericUpDown
            Me.checkBoxLimitActors = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.checkBoxShowRatings = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.checkBoxAdditionalInfo = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.checkBoxShowAudioFormat = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.checkBoxUseShortDesc = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.tabPageMapChannels = New MediaPortal.UserInterface.Controls.MPTabPage
            Me.labelNote = New MediaPortal.UserInterface.Controls.MPLabel
            Me.groupBoxMapping = New MediaPortal.UserInterface.Controls.MPGroupBox
            Me.ButtonEPGgrab = New System.Windows.Forms.Button
            Me.panelTimeSpan = New System.Windows.Forms.Panel
            Me.maskedTextBoxTimeStart = New System.Windows.Forms.MaskedTextBox
            Me.label1 = New MediaPortal.UserInterface.Controls.MPLabel
            Me.maskedTextBoxTimeEnd = New System.Windows.Forms.MaskedTextBox
            Me.treeViewMpChannels = New System.Windows.Forms.TreeView
            Me.treeViewTvMStations = New System.Windows.Forms.TreeView
            Me.imageListTvmStations = New System.Windows.Forms.ImageList(Me.components)
            Me.listView1 = New MediaPortal.UserInterface.Controls.MPListView
            Me.columnHeader1 = New System.Windows.Forms.ColumnHeader
            Me.listView2 = New MediaPortal.UserInterface.Controls.MPListView
            Me.columnHeader2 = New System.Windows.Forms.ColumnHeader
            Me.tabClickfinderPG = New System.Windows.Forms.TabPage
            Me.GroupBox3 = New System.Windows.Forms.GroupBox
            Me.Label7 = New System.Windows.Forms.Label
            Me.Label6 = New System.Windows.Forms.Label
            Me.tbMPThumbs = New System.Windows.Forms.TextBox
            Me.Label5 = New System.Windows.Forms.Label
            Me.LinkClickfinderPG = New System.Windows.Forms.LinkLabel
            Me.CheckBoxClickfinderPG = New MediaPortal.UserInterface.Controls.MPCheckBox
            Me.Label4 = New System.Windows.Forms.Label
            Me.fileDialogDb = New System.Windows.Forms.OpenFileDialog
            Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
            Me.tabControlTvMovie.SuspendLayout()
            Me.tabPageSettings.SuspendLayout()
            Me.groupBoxEnableTvMovie.SuspendLayout()
            Me.GroupBox1.SuspendLayout()
            Me.groupBoxInstallMethod.SuspendLayout()
            Me.groupBoxImportTime.SuspendLayout()
            Me.tabPageImportOptions.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.groupBoxDescriptions.SuspendLayout()
            CType(Me.numericUpDownActorCount, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tabPageMapChannels.SuspendLayout()
            Me.groupBoxMapping.SuspendLayout()
            Me.panelTimeSpan.SuspendLayout()
            Me.tabClickfinderPG.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            Me.SuspendLayout()
            '
            'openFileDialog
            '
            Me.openFileDialog.DefaultExt = "mdb"
            Me.openFileDialog.FileName = "tvdaten.mdb"
            Me.openFileDialog.Filter = "TV Movie Database|*.mdb"
            Me.openFileDialog.RestoreDirectory = True
            '
            'tabControlTvMovie
            '
            Me.tabControlTvMovie.AllowDrop = True
            Me.tabControlTvMovie.AllowReorderTabs = False
            Me.tabControlTvMovie.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tabControlTvMovie.Controls.Add(Me.tabPageSettings)
            Me.tabControlTvMovie.Controls.Add(Me.tabPageImportOptions)
            Me.tabControlTvMovie.Controls.Add(Me.tabPageMapChannels)
            Me.tabControlTvMovie.Controls.Add(Me.tabClickfinderPG)
            Me.tabControlTvMovie.Location = New System.Drawing.Point(0, 0)
            Me.tabControlTvMovie.Name = "tabControlTvMovie"
            Me.tabControlTvMovie.SelectedIndex = 0
            Me.tabControlTvMovie.Size = New System.Drawing.Size(464, 384)
            Me.tabControlTvMovie.TabIndex = 0
            '
            'tabPageSettings
            '
            Me.tabPageSettings.Controls.Add(Me.linkLabelInfo)
            Me.tabPageSettings.Controls.Add(Me.groupBoxEnableTvMovie)
            Me.tabPageSettings.Location = New System.Drawing.Point(4, 22)
            Me.tabPageSettings.Name = "tabPageSettings"
            Me.tabPageSettings.Padding = New System.Windows.Forms.Padding(3)
            Me.tabPageSettings.Size = New System.Drawing.Size(456, 358)
            Me.tabPageSettings.TabIndex = 1
            Me.tabPageSettings.Text = "Settings"
            Me.tabPageSettings.UseVisualStyleBackColor = True
            '
            'linkLabelInfo
            '
            Me.linkLabelInfo.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.linkLabelInfo.AutoSize = True
            Me.linkLabelInfo.LinkArea = New System.Windows.Forms.LinkArea(0, 20)
            Me.linkLabelInfo.Location = New System.Drawing.Point(32, 338)
            Me.linkLabelInfo.Name = "linkLabelInfo"
            Me.linkLabelInfo.Size = New System.Drawing.Size(352, 17)
            Me.linkLabelInfo.TabIndex = 5
            Me.linkLabelInfo.TabStop = True
            Me.linkLabelInfo.Text = "TV Movie ClickFinder is an EPG application for German TV channels."
            Me.linkLabelInfo.UseCompatibleTextRendering = True
            '
            'groupBoxEnableTvMovie
            '
            Me.groupBoxEnableTvMovie.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.groupBoxEnableTvMovie.Controls.Add(Me.GroupBox1)
            Me.groupBoxEnableTvMovie.Controls.Add(Me.groupBoxInstallMethod)
            Me.groupBoxEnableTvMovie.Controls.Add(Me.groupBoxImportTime)
            Me.groupBoxEnableTvMovie.Controls.Add(Me.checkBoxEnableImport)
            Me.groupBoxEnableTvMovie.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.groupBoxEnableTvMovie.Location = New System.Drawing.Point(16, 8)
            Me.groupBoxEnableTvMovie.Name = "groupBoxEnableTvMovie"
            Me.groupBoxEnableTvMovie.Size = New System.Drawing.Size(424, 327)
            Me.groupBoxEnableTvMovie.TabIndex = 0
            Me.groupBoxEnableTvMovie.TabStop = False
            Me.groupBoxEnableTvMovie.Text = "TV Movie ClickFinder EPG importer"
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Linklabel_EpSc)
            Me.GroupBox1.Controls.Add(Me.CheckBoxEpSc)
            Me.GroupBox1.Controls.Add(Me.checkBoxRunHidden)
            Me.GroupBox1.Controls.Add(Me.tbRunAppAfter)
            Me.GroupBox1.Controls.Add(Me.Label2)
            Me.GroupBox1.Controls.Add(Me.BTAppBrowse)
            Me.GroupBox1.Location = New System.Drawing.Point(16, 138)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(402, 67)
            Me.GroupBox1.TabIndex = 67
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Run application after import"
            '
            'Linklabel_EpSc
            '
            Me.Linklabel_EpSc.AutoSize = True
            Me.Linklabel_EpSc.Location = New System.Drawing.Point(186, 46)
            Me.Linklabel_EpSc.Name = "Linklabel_EpSc"
            Me.Linklabel_EpSc.Size = New System.Drawing.Size(91, 13)
            Me.Linklabel_EpSc.TabIndex = 73
            Me.Linklabel_EpSc.TabStop = True
            Me.Linklabel_EpSc.Text = "EpisodenScanner"
            '
            'CheckBoxEpSc
            '
            Me.CheckBoxEpSc.AutoSize = True
            Me.CheckBoxEpSc.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.CheckBoxEpSc.Location = New System.Drawing.Point(159, 44)
            Me.CheckBoxEpSc.Name = "CheckBoxEpSc"
            Me.CheckBoxEpSc.Size = New System.Drawing.Size(32, 17)
            Me.CheckBoxEpSc.TabIndex = 68
            Me.CheckBoxEpSc.Text = "Is"
            Me.CheckBoxEpSc.UseVisualStyleBackColor = True
            '
            'checkBoxRunHidden
            '
            Me.checkBoxRunHidden.AutoSize = True
            Me.checkBoxRunHidden.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxRunHidden.Location = New System.Drawing.Point(15, 44)
            Me.checkBoxRunHidden.Name = "checkBoxRunHidden"
            Me.checkBoxRunHidden.Size = New System.Drawing.Size(133, 17)
            Me.checkBoxRunHidden.TabIndex = 67
            Me.checkBoxRunHidden.Text = "Run application hidden"
            Me.checkBoxRunHidden.UseVisualStyleBackColor = True
            '
            'tbRunAppAfter
            '
            Me.tbRunAppAfter.Location = New System.Drawing.Point(96, 17)
            Me.tbRunAppAfter.Name = "tbRunAppAfter"
            Me.tbRunAppAfter.Size = New System.Drawing.Size(264, 20)
            Me.tbRunAppAfter.TabIndex = 6
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(12, 20)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(83, 13)
            Me.Label2.TabIndex = 66
            Me.Label2.Text = "Application path"
            '
            'BTAppBrowse
            '
            Me.BTAppBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BTAppBrowse.Location = New System.Drawing.Point(361, 17)
            Me.BTAppBrowse.Name = "BTAppBrowse"
            Me.BTAppBrowse.Size = New System.Drawing.Size(21, 20)
            Me.BTAppBrowse.TabIndex = 66
            Me.BTAppBrowse.Text = "..."
            Me.BTAppBrowse.UseVisualStyleBackColor = True
            '
            'groupBoxInstallMethod
            '
            Me.groupBoxInstallMethod.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.groupBoxInstallMethod.Controls.Add(Me.buttonBrowse)
            Me.groupBoxInstallMethod.Controls.Add(Me.lbDbPath)
            Me.groupBoxInstallMethod.Controls.Add(Me.rbManual)
            Me.groupBoxInstallMethod.Controls.Add(Me.rbLocal)
            Me.groupBoxInstallMethod.Controls.Add(Me.tbDbPath)
            Me.groupBoxInstallMethod.Enabled = False
            Me.groupBoxInstallMethod.Location = New System.Drawing.Point(16, 55)
            Me.groupBoxInstallMethod.Name = "groupBoxInstallMethod"
            Me.groupBoxInstallMethod.Size = New System.Drawing.Size(391, 77)
            Me.groupBoxInstallMethod.TabIndex = 61
            Me.groupBoxInstallMethod.TabStop = False
            Me.groupBoxInstallMethod.Text = "Installation type"
            '
            'buttonBrowse
            '
            Me.buttonBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.buttonBrowse.Location = New System.Drawing.Point(351, 45)
            Me.buttonBrowse.Name = "buttonBrowse"
            Me.buttonBrowse.Size = New System.Drawing.Size(21, 20)
            Me.buttonBrowse.TabIndex = 65
            Me.buttonBrowse.Text = "..."
            Me.buttonBrowse.UseVisualStyleBackColor = True
            '
            'lbDbPath
            '
            Me.lbDbPath.AutoSize = True
            Me.lbDbPath.Location = New System.Drawing.Point(12, 48)
            Me.lbDbPath.Name = "lbDbPath"
            Me.lbDbPath.Size = New System.Drawing.Size(77, 13)
            Me.lbDbPath.TabIndex = 64
            Me.lbDbPath.Text = "Database path"
            '
            'rbManual
            '
            Me.rbManual.AutoSize = True
            Me.rbManual.Location = New System.Drawing.Point(234, 19)
            Me.rbManual.Name = "rbManual"
            Me.rbManual.Size = New System.Drawing.Size(137, 17)
            Me.rbManual.TabIndex = 63
            Me.rbManual.Text = "Manually import from file"
            Me.rbManual.UseVisualStyleBackColor = True
            '
            'rbLocal
            '
            Me.rbLocal.AutoSize = True
            Me.rbLocal.Checked = True
            Me.rbLocal.Location = New System.Drawing.Point(15, 19)
            Me.rbLocal.Name = "rbLocal"
            Me.rbLocal.Size = New System.Drawing.Size(147, 17)
            Me.rbLocal.TabIndex = 62
            Me.rbLocal.TabStop = True
            Me.rbLocal.Text = "Clickfinder installed locally"
            Me.rbLocal.UseVisualStyleBackColor = True
            '
            'tbDbPath
            '
            Me.tbDbPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbDbPath.Enabled = False
            Me.tbDbPath.Location = New System.Drawing.Point(98, 45)
            Me.tbDbPath.Name = "tbDbPath"
            Me.tbDbPath.Size = New System.Drawing.Size(245, 20)
            Me.tbDbPath.TabIndex = 61
            '
            'groupBoxImportTime
            '
            Me.groupBoxImportTime.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.groupBoxImportTime.Controls.Add(Me.tbImportStartTime)
            Me.groupBoxImportTime.Controls.Add(Me.MpCheckBoxStartImportAtTime)
            Me.groupBoxImportTime.Controls.Add(Me.progressBarImportTotal)
            Me.groupBoxImportTime.Controls.Add(Me.buttonImportNow)
            Me.groupBoxImportTime.Controls.Add(Me.checkBoxSlowImport)
            Me.groupBoxImportTime.Controls.Add(Me.radioButton7d)
            Me.groupBoxImportTime.Controls.Add(Me.radioButton2d)
            Me.groupBoxImportTime.Controls.Add(Me.radioButton24h)
            Me.groupBoxImportTime.Controls.Add(Me.radioButton12h)
            Me.groupBoxImportTime.Controls.Add(Me.radioButton6h)
            Me.groupBoxImportTime.Enabled = False
            Me.groupBoxImportTime.Location = New System.Drawing.Point(16, 211)
            Me.groupBoxImportTime.Name = "groupBoxImportTime"
            Me.groupBoxImportTime.Size = New System.Drawing.Size(391, 110)
            Me.groupBoxImportTime.TabIndex = 2
            Me.groupBoxImportTime.TabStop = False
            Me.groupBoxImportTime.Text = "Import newer database after"
            '
            'tbImportStartTime
            '
            Me.tbImportStartTime.Location = New System.Drawing.Point(108, 40)
            Me.tbImportStartTime.Mask = "00:00"
            Me.tbImportStartTime.Name = "tbImportStartTime"
            Me.tbImportStartTime.Size = New System.Drawing.Size(49, 20)
            Me.tbImportStartTime.TabIndex = 71
            Me.tbImportStartTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            Me.tbImportStartTime.ValidatingType = GetType(Date)
            '
            'MpCheckBoxStartImportAtTime
            '
            Me.MpCheckBoxStartImportAtTime.AutoSize = True
            Me.MpCheckBoxStartImportAtTime.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.MpCheckBoxStartImportAtTime.Location = New System.Drawing.Point(15, 41)
            Me.MpCheckBoxStartImportAtTime.Name = "MpCheckBoxStartImportAtTime"
            Me.MpCheckBoxStartImportAtTime.Size = New System.Drawing.Size(87, 17)
            Me.MpCheckBoxStartImportAtTime.TabIndex = 70
            Me.MpCheckBoxStartImportAtTime.Text = "start import at"
            Me.MpCheckBoxStartImportAtTime.UseVisualStyleBackColor = True
            '
            'progressBarImportTotal
            '
            Me.progressBarImportTotal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.progressBarImportTotal.Location = New System.Drawing.Point(98, 68)
            Me.progressBarImportTotal.Name = "progressBarImportTotal"
            Me.progressBarImportTotal.Size = New System.Drawing.Size(274, 21)
            Me.progressBarImportTotal.TabIndex = 61
            '
            'buttonImportNow
            '
            Me.buttonImportNow.Location = New System.Drawing.Point(15, 68)
            Me.buttonImportNow.Name = "buttonImportNow"
            Me.buttonImportNow.Size = New System.Drawing.Size(75, 21)
            Me.buttonImportNow.TabIndex = 60
            Me.buttonImportNow.Text = "Import now!"
            Me.buttonImportNow.UseVisualStyleBackColor = True
            '
            'checkBoxSlowImport
            '
            Me.checkBoxSlowImport.AutoSize = True
            Me.checkBoxSlowImport.Checked = True
            Me.checkBoxSlowImport.CheckState = System.Windows.Forms.CheckState.Checked
            Me.checkBoxSlowImport.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxSlowImport.Location = New System.Drawing.Point(16, 91)
            Me.checkBoxSlowImport.Name = "checkBoxSlowImport"
            Me.checkBoxSlowImport.Size = New System.Drawing.Size(245, 17)
            Me.checkBoxSlowImport.TabIndex = 7
            Me.checkBoxSlowImport.Text = "Slower import (less cpu - better while using MP)"
            Me.checkBoxSlowImport.UseVisualStyleBackColor = True
            '
            'radioButton7d
            '
            Me.radioButton7d.AutoSize = True
            Me.radioButton7d.Location = New System.Drawing.Point(299, 18)
            Me.radioButton7d.Name = "radioButton7d"
            Me.radioButton7d.Size = New System.Drawing.Size(56, 17)
            Me.radioButton7d.TabIndex = 4
            Me.radioButton7d.Text = "7 days"
            Me.radioButton7d.UseVisualStyleBackColor = True
            '
            'radioButton2d
            '
            Me.radioButton2d.AutoSize = True
            Me.radioButton2d.Location = New System.Drawing.Point(234, 18)
            Me.radioButton2d.Name = "radioButton2d"
            Me.radioButton2d.Size = New System.Drawing.Size(56, 17)
            Me.radioButton2d.TabIndex = 3
            Me.radioButton2d.Text = "2 days"
            Me.radioButton2d.UseVisualStyleBackColor = True
            '
            'radioButton24h
            '
            Me.radioButton24h.AutoSize = True
            Me.radioButton24h.Checked = True
            Me.radioButton24h.Location = New System.Drawing.Point(159, 18)
            Me.radioButton24h.Name = "radioButton24h"
            Me.radioButton24h.Size = New System.Drawing.Size(66, 17)
            Me.radioButton24h.TabIndex = 2
            Me.radioButton24h.TabStop = True
            Me.radioButton24h.Text = "24 hours"
            Me.radioButton24h.UseVisualStyleBackColor = True
            '
            'radioButton12h
            '
            Me.radioButton12h.AutoSize = True
            Me.radioButton12h.Location = New System.Drawing.Point(84, 18)
            Me.radioButton12h.Name = "radioButton12h"
            Me.radioButton12h.Size = New System.Drawing.Size(66, 17)
            Me.radioButton12h.TabIndex = 1
            Me.radioButton12h.Text = "12 hours"
            Me.radioButton12h.UseVisualStyleBackColor = True
            '
            'radioButton6h
            '
            Me.radioButton6h.AutoSize = True
            Me.radioButton6h.Location = New System.Drawing.Point(15, 18)
            Me.radioButton6h.Name = "radioButton6h"
            Me.radioButton6h.Size = New System.Drawing.Size(60, 17)
            Me.radioButton6h.TabIndex = 0
            Me.radioButton6h.TabStop = True
            Me.radioButton6h.Text = "6 hours"
            Me.radioButton6h.UseVisualStyleBackColor = True
            '
            'checkBoxEnableImport
            '
            Me.checkBoxEnableImport.AutoSize = True
            Me.checkBoxEnableImport.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxEnableImport.Location = New System.Drawing.Point(16, 20)
            Me.checkBoxEnableImport.Name = "checkBoxEnableImport"
            Me.checkBoxEnableImport.Size = New System.Drawing.Size(207, 17)
            Me.checkBoxEnableImport.TabIndex = 0
            Me.checkBoxEnableImport.Text = "Enable import from TV Movie database"
            Me.checkBoxEnableImport.UseVisualStyleBackColor = True
            '
            'tabPageImportOptions
            '
            Me.tabPageImportOptions.Controls.Add(Me.GroupBox2)
            Me.tabPageImportOptions.Controls.Add(Me.groupBoxDescriptions)
            Me.tabPageImportOptions.Location = New System.Drawing.Point(4, 22)
            Me.tabPageImportOptions.Name = "tabPageImportOptions"
            Me.tabPageImportOptions.Size = New System.Drawing.Size(456, 358)
            Me.tabPageImportOptions.TabIndex = 2
            Me.tabPageImportOptions.Text = "Import options"
            Me.tabPageImportOptions.UseVisualStyleBackColor = True
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.CheckBoxTheTvDb)
            Me.GroupBox2.Controls.Add(Me.ButtonSeriesMapping)
            Me.GroupBox2.Controls.Add(Me.CheckBoxVideoDB)
            Me.GroupBox2.Controls.Add(Me.CheckBoxMyFilms)
            Me.GroupBox2.Controls.Add(Me.CheckBoxMovingPictures)
            Me.GroupBox2.Controls.Add(Me.CheckBoxTvSeries)
            Me.GroupBox2.Controls.Add(Me.ButtonBrowseMPDatabases)
            Me.GroupBox2.Controls.Add(Me.Label3)
            Me.GroupBox2.Controls.Add(Me.tbMPDatabasePath)
            Me.GroupBox2.Location = New System.Drawing.Point(16, 194)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(440, 153)
            Me.GroupBox2.TabIndex = 7
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Import additional Infos from MediaPortal databases"
            '
            'CheckBoxTheTvDb
            '
            Me.CheckBoxTheTvDb.AutoSize = True
            Me.CheckBoxTheTvDb.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.CheckBoxTheTvDb.Location = New System.Drawing.Point(264, 63)
            Me.CheckBoxTheTvDb.Name = "CheckBoxTheTvDb"
            Me.CheckBoxTheTvDb.Size = New System.Drawing.Size(113, 17)
            Me.CheckBoxTheTvDb.TabIndex = 74
            Me.CheckBoxTheTvDb.Text = "use TheTvDb.com"
            Me.CheckBoxTheTvDb.UseVisualStyleBackColor = True
            '
            'ButtonSeriesMapping
            '
            Me.ButtonSeriesMapping.Location = New System.Drawing.Point(116, 59)
            Me.ButtonSeriesMapping.Name = "ButtonSeriesMapping"
            Me.ButtonSeriesMapping.Size = New System.Drawing.Size(96, 21)
            Me.ButtonSeriesMapping.TabIndex = 73
            Me.ButtonSeriesMapping.Text = "SeriesMapping"
            Me.ButtonSeriesMapping.UseVisualStyleBackColor = True
            '
            'CheckBoxVideoDB
            '
            Me.CheckBoxVideoDB.AutoSize = True
            Me.CheckBoxVideoDB.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.CheckBoxVideoDB.Location = New System.Drawing.Point(16, 110)
            Me.CheckBoxVideoDB.Name = "CheckBoxVideoDB"
            Me.CheckBoxVideoDB.Size = New System.Drawing.Size(97, 17)
            Me.CheckBoxVideoDB.TabIndex = 72
            Me.CheckBoxVideoDB.Text = "VideoDatabase"
            Me.CheckBoxVideoDB.UseVisualStyleBackColor = True
            '
            'CheckBoxMyFilms
            '
            Me.CheckBoxMyFilms.AutoSize = True
            Me.CheckBoxMyFilms.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.CheckBoxMyFilms.Location = New System.Drawing.Point(279, 110)
            Me.CheckBoxMyFilms.Name = "CheckBoxMyFilms"
            Me.CheckBoxMyFilms.Size = New System.Drawing.Size(148, 17)
            Me.CheckBoxMyFilms.TabIndex = 71
            Me.CheckBoxMyFilms.Text = "My Films (not included yet)"
            Me.CheckBoxMyFilms.UseVisualStyleBackColor = True
            '
            'CheckBoxMovingPictures
            '
            Me.CheckBoxMovingPictures.AutoSize = True
            Me.CheckBoxMovingPictures.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.CheckBoxMovingPictures.Location = New System.Drawing.Point(158, 110)
            Me.CheckBoxMovingPictures.Name = "CheckBoxMovingPictures"
            Me.CheckBoxMovingPictures.Size = New System.Drawing.Size(97, 17)
            Me.CheckBoxMovingPictures.TabIndex = 70
            Me.CheckBoxMovingPictures.Text = "MovingPictures"
            Me.CheckBoxMovingPictures.UseVisualStyleBackColor = True
            '
            'CheckBoxTvSeries
            '
            Me.CheckBoxTvSeries.AutoSize = True
            Me.CheckBoxTvSeries.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.CheckBoxTvSeries.Location = New System.Drawing.Point(16, 63)
            Me.CheckBoxTvSeries.Name = "CheckBoxTvSeries"
            Me.CheckBoxTvSeries.Size = New System.Drawing.Size(85, 17)
            Me.CheckBoxTvSeries.TabIndex = 69
            Me.CheckBoxTvSeries.Text = "MP-TvSeries"
            Me.CheckBoxTvSeries.UseVisualStyleBackColor = True
            '
            'ButtonBrowseMPDatabases
            '
            Me.ButtonBrowseMPDatabases.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ButtonBrowseMPDatabases.Location = New System.Drawing.Point(406, 25)
            Me.ButtonBrowseMPDatabases.Name = "ButtonBrowseMPDatabases"
            Me.ButtonBrowseMPDatabases.Size = New System.Drawing.Size(21, 20)
            Me.ButtonBrowseMPDatabases.TabIndex = 68
            Me.ButtonBrowseMPDatabases.Text = "..."
            Me.ButtonBrowseMPDatabases.UseVisualStyleBackColor = True
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(13, 29)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(101, 13)
            Me.Label3.TabIndex = 67
            Me.Label3.Text = "MP Databases path"
            '
            'tbMPDatabasePath
            '
            Me.tbMPDatabasePath.Location = New System.Drawing.Point(120, 26)
            Me.tbMPDatabasePath.Name = "tbMPDatabasePath"
            Me.tbMPDatabasePath.Size = New System.Drawing.Size(259, 20)
            Me.tbMPDatabasePath.TabIndex = 6
            '
            'groupBoxDescriptions
            '
            Me.groupBoxDescriptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.groupBoxDescriptions.Controls.Add(Me.checkBoxShowRepeat)
            Me.groupBoxDescriptions.Controls.Add(Me.checkBoxShowLive)
            Me.groupBoxDescriptions.Controls.Add(Me.numericUpDownActorCount)
            Me.groupBoxDescriptions.Controls.Add(Me.checkBoxLimitActors)
            Me.groupBoxDescriptions.Controls.Add(Me.checkBoxShowRatings)
            Me.groupBoxDescriptions.Controls.Add(Me.checkBoxAdditionalInfo)
            Me.groupBoxDescriptions.Controls.Add(Me.checkBoxShowAudioFormat)
            Me.groupBoxDescriptions.Controls.Add(Me.checkBoxUseShortDesc)
            Me.groupBoxDescriptions.Enabled = False
            Me.groupBoxDescriptions.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.groupBoxDescriptions.Location = New System.Drawing.Point(16, 8)
            Me.groupBoxDescriptions.Name = "groupBoxDescriptions"
            Me.groupBoxDescriptions.Size = New System.Drawing.Size(424, 180)
            Me.groupBoxDescriptions.TabIndex = 5
            Me.groupBoxDescriptions.TabStop = False
            Me.groupBoxDescriptions.Text = "Descriptions"
            '
            'checkBoxShowRepeat
            '
            Me.checkBoxShowRepeat.AutoSize = True
            Me.checkBoxShowRepeat.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxShowRepeat.Location = New System.Drawing.Point(16, 152)
            Me.checkBoxShowRepeat.Name = "checkBoxShowRepeat"
            Me.checkBoxShowRepeat.Size = New System.Drawing.Size(178, 17)
            Me.checkBoxShowRepeat.TabIndex = 8
            Me.checkBoxShowRepeat.Text = "Append ""(Wdh.)"" to program title"
            Me.checkBoxShowRepeat.UseVisualStyleBackColor = True
            '
            'checkBoxShowLive
            '
            Me.checkBoxShowLive.AutoSize = True
            Me.checkBoxShowLive.Checked = True
            Me.checkBoxShowLive.CheckState = System.Windows.Forms.CheckState.Checked
            Me.checkBoxShowLive.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxShowLive.Location = New System.Drawing.Point(16, 129)
            Me.checkBoxShowLive.Name = "checkBoxShowLive"
            Me.checkBoxShowLive.Size = New System.Drawing.Size(175, 17)
            Me.checkBoxShowLive.TabIndex = 7
            Me.checkBoxShowLive.Text = "Append ""(LIVE)"" to program title"
            Me.checkBoxShowLive.UseVisualStyleBackColor = True
            '
            'numericUpDownActorCount
            '
            Me.numericUpDownActorCount.Location = New System.Drawing.Point(218, 63)
            Me.numericUpDownActorCount.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
            Me.numericUpDownActorCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.numericUpDownActorCount.Name = "numericUpDownActorCount"
            Me.numericUpDownActorCount.Size = New System.Drawing.Size(37, 20)
            Me.numericUpDownActorCount.TabIndex = 6
            Me.numericUpDownActorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            Me.numericUpDownActorCount.Value = New Decimal(New Integer() {5, 0, 0, 0})
            '
            'checkBoxLimitActors
            '
            Me.checkBoxLimitActors.AutoSize = True
            Me.checkBoxLimitActors.Checked = True
            Me.checkBoxLimitActors.CheckState = System.Windows.Forms.CheckState.Checked
            Me.checkBoxLimitActors.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxLimitActors.Location = New System.Drawing.Point(31, 63)
            Me.checkBoxLimitActors.Name = "checkBoxLimitActors"
            Me.checkBoxLimitActors.Size = New System.Drawing.Size(181, 17)
            Me.checkBoxLimitActors.TabIndex = 4
            Me.checkBoxLimitActors.Text = "Limit actors - show a maximum of "
            Me.checkBoxLimitActors.UseVisualStyleBackColor = True
            '
            'checkBoxShowRatings
            '
            Me.checkBoxShowRatings.AutoSize = True
            Me.checkBoxShowRatings.Checked = True
            Me.checkBoxShowRatings.CheckState = System.Windows.Forms.CheckState.Checked
            Me.checkBoxShowRatings.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxShowRatings.Location = New System.Drawing.Point(16, 86)
            Me.checkBoxShowRatings.Name = "checkBoxShowRatings"
            Me.checkBoxShowRatings.Size = New System.Drawing.Size(126, 17)
            Me.checkBoxShowRatings.TabIndex = 3
            Me.checkBoxShowRatings.Text = "Show program ratings"
            Me.checkBoxShowRatings.UseVisualStyleBackColor = True
            '
            'checkBoxAdditionalInfo
            '
            Me.checkBoxAdditionalInfo.AutoSize = True
            Me.checkBoxAdditionalInfo.Checked = True
            Me.checkBoxAdditionalInfo.CheckState = System.Windows.Forms.CheckState.Checked
            Me.checkBoxAdditionalInfo.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxAdditionalInfo.Location = New System.Drawing.Point(16, 40)
            Me.checkBoxAdditionalInfo.Name = "checkBoxAdditionalInfo"
            Me.checkBoxAdditionalInfo.Size = New System.Drawing.Size(294, 17)
            Me.checkBoxAdditionalInfo.TabIndex = 1
            Me.checkBoxAdditionalInfo.Text = "Display additional info like Episode, FSK, Year, Actors etc"
            Me.checkBoxAdditionalInfo.UseVisualStyleBackColor = True
            '
            'checkBoxShowAudioFormat
            '
            Me.checkBoxShowAudioFormat.AutoSize = True
            Me.checkBoxShowAudioFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxShowAudioFormat.Location = New System.Drawing.Point(16, 106)
            Me.checkBoxShowAudioFormat.Name = "checkBoxShowAudioFormat"
            Me.checkBoxShowAudioFormat.Size = New System.Drawing.Size(117, 17)
            Me.checkBoxShowAudioFormat.TabIndex = 2
            Me.checkBoxShowAudioFormat.Text = "Show audio formats"
            Me.checkBoxShowAudioFormat.UseVisualStyleBackColor = True
            '
            'checkBoxUseShortDesc
            '
            Me.checkBoxUseShortDesc.AutoSize = True
            Me.checkBoxUseShortDesc.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.checkBoxUseShortDesc.Location = New System.Drawing.Point(16, 20)
            Me.checkBoxUseShortDesc.Name = "checkBoxUseShortDesc"
            Me.checkBoxUseShortDesc.Size = New System.Drawing.Size(204, 17)
            Me.checkBoxUseShortDesc.TabIndex = 0
            Me.checkBoxUseShortDesc.Text = "Use short program descriptions (faster)"
            Me.checkBoxUseShortDesc.UseVisualStyleBackColor = True
            '
            'tabPageMapChannels
            '
            Me.tabPageMapChannels.Controls.Add(Me.labelNote)
            Me.tabPageMapChannels.Controls.Add(Me.groupBoxMapping)
            Me.tabPageMapChannels.Location = New System.Drawing.Point(4, 22)
            Me.tabPageMapChannels.Name = "tabPageMapChannels"
            Me.tabPageMapChannels.Padding = New System.Windows.Forms.Padding(3)
            Me.tabPageMapChannels.Size = New System.Drawing.Size(456, 358)
            Me.tabPageMapChannels.TabIndex = 0
            Me.tabPageMapChannels.Text = "Map channels"
            Me.tabPageMapChannels.UseVisualStyleBackColor = True
            '
            'labelNote
            '
            Me.labelNote.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.labelNote.AutoSize = True
            Me.labelNote.Location = New System.Drawing.Point(217, 342)
            Me.labelNote.Name = "labelNote"
            Me.labelNote.Size = New System.Drawing.Size(223, 13)
            Me.labelNote.TabIndex = 1
            Me.labelNote.Text = "Note: Use doubleclick to map/unmap stations"
            Me.labelNote.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'groupBoxMapping
            '
            Me.groupBoxMapping.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.groupBoxMapping.Controls.Add(Me.ButtonEPGgrab)
            Me.groupBoxMapping.Controls.Add(Me.panelTimeSpan)
            Me.groupBoxMapping.Controls.Add(Me.treeViewMpChannels)
            Me.groupBoxMapping.Controls.Add(Me.treeViewTvMStations)
            Me.groupBoxMapping.Controls.Add(Me.listView1)
            Me.groupBoxMapping.Controls.Add(Me.listView2)
            Me.groupBoxMapping.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.groupBoxMapping.Location = New System.Drawing.Point(16, 8)
            Me.groupBoxMapping.Name = "groupBoxMapping"
            Me.groupBoxMapping.Size = New System.Drawing.Size(424, 331)
            Me.groupBoxMapping.TabIndex = 0
            Me.groupBoxMapping.TabStop = False
            Me.groupBoxMapping.Text = "Map channels to TV Movie stations"
            '
            'ButtonEPGgrab
            '
            Me.ButtonEPGgrab.Location = New System.Drawing.Point(178, 336)
            Me.ButtonEPGgrab.Name = "ButtonEPGgrab"
            Me.ButtonEPGgrab.Size = New System.Drawing.Size(249, 27)
            Me.ButtonEPGgrab.TabIndex = 5
            Me.ButtonEPGgrab.Text = "Enable EPG grabbing for not mapped channels"
            Me.ButtonEPGgrab.UseVisualStyleBackColor = True
            '
            'panelTimeSpan
            '
            Me.panelTimeSpan.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.panelTimeSpan.AutoSize = True
            Me.panelTimeSpan.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.panelTimeSpan.Controls.Add(Me.maskedTextBoxTimeStart)
            Me.panelTimeSpan.Controls.Add(Me.label1)
            Me.panelTimeSpan.Controls.Add(Me.maskedTextBoxTimeEnd)
            Me.panelTimeSpan.Location = New System.Drawing.Point(14, 298)
            Me.panelTimeSpan.Name = "panelTimeSpan"
            Me.panelTimeSpan.Size = New System.Drawing.Size(139, 27)
            Me.panelTimeSpan.TabIndex = 4
            Me.panelTimeSpan.Visible = False
            '
            'maskedTextBoxTimeStart
            '
            Me.maskedTextBoxTimeStart.AsciiOnly = True
            Me.maskedTextBoxTimeStart.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals
            Me.maskedTextBoxTimeStart.Location = New System.Drawing.Point(0, 4)
            Me.maskedTextBoxTimeStart.Mask = "90:00"
            Me.maskedTextBoxTimeStart.Name = "maskedTextBoxTimeStart"
            Me.maskedTextBoxTimeStart.PromptChar = Global.Microsoft.VisualBasic.ChrW(48)
            Me.maskedTextBoxTimeStart.RejectInputOnFirstFailure = True
            Me.maskedTextBoxTimeStart.Size = New System.Drawing.Size(56, 20)
            Me.maskedTextBoxTimeStart.TabIndex = 0
            Me.maskedTextBoxTimeStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            Me.maskedTextBoxTimeStart.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals
            Me.maskedTextBoxTimeStart.ValidatingType = GetType(Date)
            '
            'label1
            '
            Me.label1.AutoSize = True
            Me.label1.Location = New System.Drawing.Point(63, 8)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(10, 13)
            Me.label1.TabIndex = 1
            Me.label1.Text = "-"
            '
            'maskedTextBoxTimeEnd
            '
            Me.maskedTextBoxTimeEnd.AsciiOnly = True
            Me.maskedTextBoxTimeEnd.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals
            Me.maskedTextBoxTimeEnd.Location = New System.Drawing.Point(80, 4)
            Me.maskedTextBoxTimeEnd.Mask = "90:00"
            Me.maskedTextBoxTimeEnd.Name = "maskedTextBoxTimeEnd"
            Me.maskedTextBoxTimeEnd.PromptChar = Global.Microsoft.VisualBasic.ChrW(48)
            Me.maskedTextBoxTimeEnd.RejectInputOnFirstFailure = True
            Me.maskedTextBoxTimeEnd.Size = New System.Drawing.Size(56, 20)
            Me.maskedTextBoxTimeEnd.TabIndex = 2
            Me.maskedTextBoxTimeEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            Me.maskedTextBoxTimeEnd.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals
            Me.maskedTextBoxTimeEnd.ValidatingType = GetType(Date)
            '
            'treeViewMpChannels
            '
            Me.treeViewMpChannels.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.treeViewMpChannels.HideSelection = False
            Me.treeViewMpChannels.Location = New System.Drawing.Point(12, 48)
            Me.treeViewMpChannels.Name = "treeViewMpChannels"
            Me.treeViewMpChannels.ShowNodeToolTips = True
            Me.treeViewMpChannels.Size = New System.Drawing.Size(216, 244)
            Me.treeViewMpChannels.Sorted = True
            Me.treeViewMpChannels.TabIndex = 1
            '
            'treeViewTvMStations
            '
            Me.treeViewTvMStations.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.treeViewTvMStations.HideSelection = False
            Me.treeViewTvMStations.ImageIndex = 0
            Me.treeViewTvMStations.ImageList = Me.imageListTvmStations
            Me.treeViewTvMStations.Indent = 35
            Me.treeViewTvMStations.ItemHeight = 24
            Me.treeViewTvMStations.Location = New System.Drawing.Point(236, 48)
            Me.treeViewTvMStations.Name = "treeViewTvMStations"
            Me.treeViewTvMStations.SelectedImageIndex = 0
            Me.treeViewTvMStations.ShowNodeToolTips = True
            Me.treeViewTvMStations.ShowPlusMinus = False
            Me.treeViewTvMStations.ShowRootLines = False
            Me.treeViewTvMStations.Size = New System.Drawing.Size(172, 244)
            Me.treeViewTvMStations.Sorted = True
            Me.treeViewTvMStations.TabIndex = 3
            '
            'imageListTvmStations
            '
            Me.imageListTvmStations.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit
            Me.imageListTvmStations.ImageSize = New System.Drawing.Size(32, 22)
            Me.imageListTvmStations.TransparentColor = System.Drawing.Color.Transparent
            '
            'listView1
            '
            Me.listView1.AllowDrop = True
            Me.listView1.AllowRowReorder = True
            Me.listView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeader1})
            Me.listView1.IsChannelListView = False
            Me.listView1.Location = New System.Drawing.Point(12, 27)
            Me.listView1.Name = "listView1"
            Me.listView1.Scrollable = False
            Me.listView1.Size = New System.Drawing.Size(216, 24)
            Me.listView1.TabIndex = 0
            Me.listView1.UseCompatibleStateImageBehavior = False
            Me.listView1.View = System.Windows.Forms.View.Details
            '
            'columnHeader1
            '
            Me.columnHeader1.Text = "TV Service channels"
            Me.columnHeader1.Width = 262
            '
            'listView2
            '
            Me.listView2.AllowDrop = True
            Me.listView2.AllowRowReorder = True
            Me.listView2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.listView2.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeader2})
            Me.listView2.IsChannelListView = False
            Me.listView2.Location = New System.Drawing.Point(236, 27)
            Me.listView2.Name = "listView2"
            Me.listView2.Scrollable = False
            Me.listView2.Size = New System.Drawing.Size(172, 24)
            Me.listView2.TabIndex = 2
            Me.listView2.UseCompatibleStateImageBehavior = False
            Me.listView2.View = System.Windows.Forms.View.Details
            '
            'columnHeader2
            '
            Me.columnHeader2.Text = "TV Movie stations"
            Me.columnHeader2.Width = 179
            '
            'tabClickfinderPG
            '
            Me.tabClickfinderPG.Controls.Add(Me.GroupBox3)
            Me.tabClickfinderPG.Location = New System.Drawing.Point(4, 22)
            Me.tabClickfinderPG.Name = "tabClickfinderPG"
            Me.tabClickfinderPG.Padding = New System.Windows.Forms.Padding(3)
            Me.tabClickfinderPG.Size = New System.Drawing.Size(456, 358)
            Me.tabClickfinderPG.TabIndex = 3
            Me.tabClickfinderPG.Text = "Clickfinder ProgramGuide"
            Me.tabClickfinderPG.UseVisualStyleBackColor = True
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.Label7)
            Me.GroupBox3.Controls.Add(Me.Label6)
            Me.GroupBox3.Controls.Add(Me.tbMPThumbs)
            Me.GroupBox3.Controls.Add(Me.Label5)
            Me.GroupBox3.Controls.Add(Me.LinkClickfinderPG)
            Me.GroupBox3.Controls.Add(Me.CheckBoxClickfinderPG)
            Me.GroupBox3.Controls.Add(Me.Label4)
            Me.GroupBox3.Location = New System.Drawing.Point(20, 14)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(430, 338)
            Me.GroupBox3.TabIndex = 73
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "Clickfinder ProgramGuide"
            '
            'Label7
            '
            Me.Label7.Location = New System.Drawing.Point(33, 125)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(372, 85)
            Me.Label7.TabIndex = 77
            Me.Label7.Text = resources.GetString("Label7.Text")
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(22, 98)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(88, 13)
            Me.Label6.TabIndex = 76
            Me.Label6.Text = "MP Thumbs path"
            '
            'tbMPThumbs
            '
            Me.tbMPThumbs.Location = New System.Drawing.Point(129, 95)
            Me.tbMPThumbs.Name = "tbMPThumbs"
            Me.tbMPThumbs.Size = New System.Drawing.Size(259, 20)
            Me.tbMPThumbs.TabIndex = 75
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(211, 57)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(0, 13)
            Me.Label5.TabIndex = 74
            '
            'LinkClickfinderPG
            '
            Me.LinkClickfinderPG.AutoSize = True
            Me.LinkClickfinderPG.Location = New System.Drawing.Point(71, 43)
            Me.LinkClickfinderPG.Name = "LinkClickfinderPG"
            Me.LinkClickfinderPG.Size = New System.Drawing.Size(126, 13)
            Me.LinkClickfinderPG.TabIndex = 72
            Me.LinkClickfinderPG.TabStop = True
            Me.LinkClickfinderPG.Text = "Clickfinder ProgramGuide"
            '
            'CheckBoxClickfinderPG
            '
            Me.CheckBoxClickfinderPG.AutoSize = True
            Me.CheckBoxClickfinderPG.FlatStyle = System.Windows.Forms.FlatStyle.Popup
            Me.CheckBoxClickfinderPG.Location = New System.Drawing.Point(25, 24)
            Me.CheckBoxClickfinderPG.Name = "CheckBoxClickfinderPG"
            Me.CheckBoxClickfinderPG.Size = New System.Drawing.Size(57, 17)
            Me.CheckBoxClickfinderPG.TabIndex = 70
            Me.CheckBoxClickfinderPG.Text = "Enable"
            Me.CheckBoxClickfinderPG.UseVisualStyleBackColor = True
            '
            'Label4
            '
            Me.Label4.Location = New System.Drawing.Point(22, 43)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(372, 32)
            Me.Label4.TabIndex = 73
            Me.Label4.Text = "If you use                                          plugin, you have to enable it" & _
                " to import additional infos. Otherwise disable this function, it increases the i" & _
                "mport time."
            '
            'fileDialogDb
            '
            Me.fileDialogDb.FileName = "TVDaten.mdb"
            Me.fileDialogDb.RestoreDirectory = True
            Me.fileDialogDb.Title = "Please enter the path to TV movie's database"
            '
            'OpenFileDialog1
            '
            Me.OpenFileDialog1.Filter = "Application (*.exe)|*.exe|All files (*.*)|*.*"
            Me.OpenFileDialog1.InitialDirectory = "Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)"
            '
            'TvMovieSetup
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.tabControlTvMovie)
            Me.Name = "TvMovieSetup"
            Me.Size = New System.Drawing.Size(467, 388)
            Me.tabControlTvMovie.ResumeLayout(False)
            Me.tabPageSettings.ResumeLayout(False)
            Me.tabPageSettings.PerformLayout()
            Me.groupBoxEnableTvMovie.ResumeLayout(False)
            Me.groupBoxEnableTvMovie.PerformLayout()
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.groupBoxInstallMethod.ResumeLayout(False)
            Me.groupBoxInstallMethod.PerformLayout()
            Me.groupBoxImportTime.ResumeLayout(False)
            Me.groupBoxImportTime.PerformLayout()
            Me.tabPageImportOptions.ResumeLayout(False)
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.groupBoxDescriptions.ResumeLayout(False)
            Me.groupBoxDescriptions.PerformLayout()
            CType(Me.numericUpDownActorCount, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tabPageMapChannels.ResumeLayout(False)
            Me.tabPageMapChannels.PerformLayout()
            Me.groupBoxMapping.ResumeLayout(False)
            Me.groupBoxMapping.PerformLayout()
            Me.panelTimeSpan.ResumeLayout(False)
            Me.panelTimeSpan.PerformLayout()
            Me.tabClickfinderPG.ResumeLayout(False)
            Me.GroupBox3.ResumeLayout(False)
            Me.GroupBox3.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

#End Region

        Private WithEvents openFileDialog As System.Windows.Forms.OpenFileDialog
        Private WithEvents tabControlTvMovie As MediaPortal.UserInterface.Controls.MPTabControl
        Private WithEvents tabPageMapChannels As MediaPortal.UserInterface.Controls.MPTabPage
        Private WithEvents labelNote As MediaPortal.UserInterface.Controls.MPLabel
        Private WithEvents groupBoxMapping As MediaPortal.UserInterface.Controls.MPGroupBox
        Private WithEvents panelTimeSpan As System.Windows.Forms.Panel
        Private WithEvents maskedTextBoxTimeStart As System.Windows.Forms.MaskedTextBox
        Private WithEvents label1 As MediaPortal.UserInterface.Controls.MPLabel
        Private WithEvents maskedTextBoxTimeEnd As System.Windows.Forms.MaskedTextBox
        Private WithEvents treeViewMpChannels As System.Windows.Forms.TreeView
        Private WithEvents treeViewTvMStations As System.Windows.Forms.TreeView
        Private WithEvents listView1 As MediaPortal.UserInterface.Controls.MPListView
        Private WithEvents columnHeader1 As System.Windows.Forms.ColumnHeader
        Private WithEvents listView2 As MediaPortal.UserInterface.Controls.MPListView
        Private WithEvents columnHeader2 As System.Windows.Forms.ColumnHeader
        Private WithEvents tabPageSettings As System.Windows.Forms.TabPage
        Private WithEvents checkBoxEnableImport As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents groupBoxEnableTvMovie As MediaPortal.UserInterface.Controls.MPGroupBox
        Private WithEvents groupBoxImportTime As System.Windows.Forms.GroupBox
        Private WithEvents radioButton24h As System.Windows.Forms.RadioButton
        Private WithEvents radioButton12h As System.Windows.Forms.RadioButton
        Private WithEvents radioButton6h As System.Windows.Forms.RadioButton
        Private WithEvents radioButton7d As System.Windows.Forms.RadioButton
        Private WithEvents radioButton2d As System.Windows.Forms.RadioButton
        Private WithEvents checkBoxSlowImport As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents linkLabelInfo As System.Windows.Forms.LinkLabel
        Private WithEvents imageListTvmStations As System.Windows.Forms.ImageList
        Public WithEvents progressBarImportTotal As System.Windows.Forms.ProgressBar
        Private WithEvents buttonImportNow As System.Windows.Forms.Button
        Private WithEvents tabPageImportOptions As System.Windows.Forms.TabPage
        Private WithEvents groupBoxDescriptions As MediaPortal.UserInterface.Controls.MPGroupBox
        Private WithEvents checkBoxLimitActors As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents checkBoxShowRatings As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents checkBoxAdditionalInfo As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents checkBoxShowAudioFormat As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents checkBoxUseShortDesc As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents groupBoxInstallMethod As System.Windows.Forms.GroupBox
        Private WithEvents rbManual As System.Windows.Forms.RadioButton
        Private WithEvents rbLocal As System.Windows.Forms.RadioButton
        Private WithEvents tbDbPath As System.Windows.Forms.TextBox
        Private WithEvents lbDbPath As System.Windows.Forms.Label
        Private WithEvents buttonBrowse As System.Windows.Forms.Button
        Private WithEvents fileDialogDb As System.Windows.Forms.OpenFileDialog
        Private WithEvents checkBoxShowRepeat As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents checkBoxShowLive As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents numericUpDownActorCount As System.Windows.Forms.NumericUpDown
        Private WithEvents BTAppBrowse As System.Windows.Forms.Button
        Private WithEvents tbRunAppAfter As System.Windows.Forms.TextBox
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Private WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents tbMPDatabasePath As System.Windows.Forms.TextBox
        Private WithEvents checkBoxRunHidden As MediaPortal.UserInterface.Controls.MPCheckBox
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Private WithEvents ButtonBrowseMPDatabases As System.Windows.Forms.Button
        Private WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Private WithEvents CheckBoxTvSeries As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents CheckBoxMyFilms As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents CheckBoxMovingPictures As MediaPortal.UserInterface.Controls.MPCheckBox
        Friend WithEvents ButtonEPGgrab As System.Windows.Forms.Button
        Friend WithEvents tabClickfinderPG As System.Windows.Forms.TabPage
        Private WithEvents CheckBoxClickfinderPG As MediaPortal.UserInterface.Controls.MPCheckBox
        Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
        Friend WithEvents LinkClickfinderPG As System.Windows.Forms.LinkLabel
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Private WithEvents CheckBoxVideoDB As MediaPortal.UserInterface.Controls.MPCheckBox
        Friend WithEvents Linklabel_EpSc As System.Windows.Forms.LinkLabel
        Private WithEvents CheckBoxEpSc As MediaPortal.UserInterface.Controls.MPCheckBox
        Friend WithEvents ButtonSeriesMapping As System.Windows.Forms.Button
        Private WithEvents CheckBoxTheTvDb As MediaPortal.UserInterface.Controls.MPCheckBox
        Private WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents tbMPThumbs As System.Windows.Forms.TextBox
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Private WithEvents MpCheckBoxStartImportAtTime As MediaPortal.UserInterface.Controls.MPCheckBox
        Friend WithEvents tbImportStartTime As System.Windows.Forms.MaskedTextBox
    End Class
End Namespace
