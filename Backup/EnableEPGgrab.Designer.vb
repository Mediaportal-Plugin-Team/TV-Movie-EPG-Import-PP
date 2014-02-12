<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEPGgrab
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.EPGgrabData = New System.Windows.Forms.DataGridView
        Me.EPGgrabEnable = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.idChannelEPGgrab = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.ChannelEPGgrab = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.EPGgrab = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.BTSave = New System.Windows.Forms.Button
        Me.CBCeck = New System.Windows.Forms.CheckBox
        Me.BTRefresh = New System.Windows.Forms.Button
        CType(Me.EPGgrabData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'EPGgrabData
        '
        Me.EPGgrabData.AllowUserToAddRows = False
        Me.EPGgrabData.AllowUserToDeleteRows = False
        Me.EPGgrabData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.EPGgrabData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.EPGgrabEnable, Me.idChannelEPGgrab, Me.ChannelEPGgrab, Me.EPGgrab})
        Me.EPGgrabData.Location = New System.Drawing.Point(12, 12)
        Me.EPGgrabData.MultiSelect = False
        Me.EPGgrabData.Name = "EPGgrabData"
        Me.EPGgrabData.RowHeadersVisible = False
        Me.EPGgrabData.Size = New System.Drawing.Size(270, 335)
        Me.EPGgrabData.TabIndex = 0
        '
        'EPGgrabEnable
        '
        Me.EPGgrabEnable.HeaderText = "enable"
        Me.EPGgrabEnable.Name = "EPGgrabEnable"
        Me.EPGgrabEnable.Width = 50
        '
        'idChannelEPGgrab
        '
        Me.idChannelEPGgrab.HeaderText = "idChannel"
        Me.idChannelEPGgrab.Name = "idChannelEPGgrab"
        Me.idChannelEPGgrab.ReadOnly = True
        Me.idChannelEPGgrab.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.idChannelEPGgrab.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.idChannelEPGgrab.Visible = False
        '
        'ChannelEPGgrab
        '
        Me.ChannelEPGgrab.HeaderText = "Channel"
        Me.ChannelEPGgrab.Name = "ChannelEPGgrab"
        Me.ChannelEPGgrab.ReadOnly = True
        Me.ChannelEPGgrab.Width = 120
        '
        'EPGgrab
        '
        Me.EPGgrab.HeaderText = "Grab EPG active"
        Me.EPGgrab.Name = "EPGgrab"
        Me.EPGgrab.Width = 80
        '
        'BTSave
        '
        Me.BTSave.Location = New System.Drawing.Point(207, 353)
        Me.BTSave.Name = "BTSave"
        Me.BTSave.Size = New System.Drawing.Size(75, 23)
        Me.BTSave.TabIndex = 1
        Me.BTSave.Text = "Save"
        Me.BTSave.UseVisualStyleBackColor = True
        '
        'CBCeck
        '
        Me.CBCeck.AutoSize = True
        Me.CBCeck.Location = New System.Drawing.Point(12, 357)
        Me.CBCeck.Name = "CBCeck"
        Me.CBCeck.Size = New System.Drawing.Size(107, 17)
        Me.CBCeck.TabIndex = 2
        Me.CBCeck.Text = "Select / deselect" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.CBCeck.UseVisualStyleBackColor = True
        '
        'BTRefresh
        '
        Me.BTRefresh.Location = New System.Drawing.Point(126, 353)
        Me.BTRefresh.Name = "BTRefresh"
        Me.BTRefresh.Size = New System.Drawing.Size(75, 23)
        Me.BTRefresh.TabIndex = 3
        Me.BTRefresh.Text = "Refresh"
        Me.BTRefresh.UseVisualStyleBackColor = True
        '
        'frmEPGgrab
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(296, 388)
        Me.Controls.Add(Me.BTRefresh)
        Me.Controls.Add(Me.CBCeck)
        Me.Controls.Add(Me.BTSave)
        Me.Controls.Add(Me.EPGgrabData)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEPGgrab"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Set epg grabing for not mapped channels"
        CType(Me.EPGgrabData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents EPGgrabData As System.Windows.Forms.DataGridView
    Friend WithEvents BTSave As System.Windows.Forms.Button
    Friend WithEvents EPGgrabEnable As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents idChannelEPGgrab As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ChannelEPGgrab As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents EPGgrab As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CBCeck As System.Windows.Forms.CheckBox
    Friend WithEvents BTRefresh As System.Windows.Forms.Button
End Class
