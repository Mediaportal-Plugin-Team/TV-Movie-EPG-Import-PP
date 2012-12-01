<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSeriesMapping
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
        Me.CBSeries = New System.Windows.Forms.ComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.tbEpgName = New System.Windows.Forms.TextBox
        Me.ButtonSave = New System.Windows.Forms.Button
        Me.DGVseries = New System.Windows.Forms.DataGridView
        Me.DGVepisodes = New System.Windows.Forms.DataGridView
        CType(Me.DGVseries, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVepisodes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CBSeries
        '
        Me.CBSeries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CBSeries.FormattingEnabled = True
        Me.CBSeries.Location = New System.Drawing.Point(78, 20)
        Me.CBSeries.Name = "CBSeries"
        Me.CBSeries.Size = New System.Drawing.Size(323, 21)
        Me.CBSeries.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 23)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(39, 13)
        Me.Label3.TabIndex = 68
        Me.Label3.Text = "Series:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 50)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(63, 13)
        Me.Label1.TabIndex = 69
        Me.Label1.Text = "EPG Name:"
        '
        'tbEpgName
        '
        Me.tbEpgName.Location = New System.Drawing.Point(78, 47)
        Me.tbEpgName.Name = "tbEpgName"
        Me.tbEpgName.Size = New System.Drawing.Size(323, 20)
        Me.tbEpgName.TabIndex = 70
        '
        'ButtonSave
        '
        Me.ButtonSave.Location = New System.Drawing.Point(333, 73)
        Me.ButtonSave.Name = "ButtonSave"
        Me.ButtonSave.Size = New System.Drawing.Size(68, 23)
        Me.ButtonSave.TabIndex = 71
        Me.ButtonSave.Text = "save"
        Me.ButtonSave.UseVisualStyleBackColor = True
        '
        'DGVseries
        '
        Me.DGVseries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVseries.Location = New System.Drawing.Point(102, 137)
        Me.DGVseries.Name = "DGVseries"
        Me.DGVseries.Size = New System.Drawing.Size(207, 270)
        Me.DGVseries.TabIndex = 72
        '
        'DGVepisodes
        '
        Me.DGVepisodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVepisodes.Location = New System.Drawing.Point(402, 137)
        Me.DGVepisodes.Name = "DGVepisodes"
        Me.DGVepisodes.Size = New System.Drawing.Size(314, 270)
        Me.DGVepisodes.TabIndex = 73
        '
        'frmSeriesMapping
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(867, 469)
        Me.Controls.Add(Me.DGVepisodes)
        Me.Controls.Add(Me.DGVseries)
        Me.Controls.Add(Me.ButtonSave)
        Me.Controls.Add(Me.tbEpgName)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.CBSeries)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSeriesMapping"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SeriesMapping"
        CType(Me.DGVseries, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVepisodes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CBSeries As System.Windows.Forms.ComboBox
    Private WithEvents Label3 As System.Windows.Forms.Label
    Private WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tbEpgName As System.Windows.Forms.TextBox
    Friend WithEvents ButtonSave As System.Windows.Forms.Button
    Friend WithEvents DGVseries As System.Windows.Forms.DataGridView
    Friend WithEvents DGVepisodes As System.Windows.Forms.DataGridView
End Class
