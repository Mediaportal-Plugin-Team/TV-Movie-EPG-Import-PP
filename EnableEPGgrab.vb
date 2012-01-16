Imports System.Collections.Generic
Imports System.Drawing
Imports TvDatabase
Imports TvLibrary.Log
Imports Gentle.Framework
Imports Gentle.Common
Public Class frmEPGgrab

    Private Sub frmEPGgrab_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        FillEPGgrabData()

    End Sub

    Private Sub FillEPGgrabData()
        Try
            EPGgrabData.Rows.Clear()

            Dim sb As New SqlBuilder(StatementType.Select, GetType(Channel))
            sb.AddOrderByField(True, "displayName")
            Dim stmt As SqlStatement = sb.GetStatement(True)
            Dim _channel As IList(Of Channel) = ObjectFactory.GetCollection(GetType(Channel), stmt.Execute())


            For i As Integer = 0 To _channel.Count - 1

                Dim sb2 As New SqlBuilder(StatementType.Select, GetType(TvMovieMapping))
                sb2.AddConstraint([Operator].Equals, "idchannel", _channel(i).IdChannel)
                Dim stmt2 As SqlStatement = sb2.GetStatement(True)
                Dim _Result As IList(Of TvMovieMapping) = ObjectFactory.GetCollection(GetType(TvMovieMapping), stmt2.Execute())

                If _Result.Count = 0 Then
                    If _channel(i).IsTv = True Then
                        If _channel(i).GrabEpg = True Then
                            EPGgrabData.Rows.Add(True, _channel(i).IdChannel, _channel(i).DisplayName, _channel(i).GrabEpg)
                        Else
                            EPGgrabData.Rows.Add(False, _channel(i).IdChannel, _channel(i).DisplayName, _channel(i).GrabEpg)
                        End If
                    End If
                End If
            Next

        Catch ex As Exception
            Log.[Error]("TVMovie: [FillEPGgrabData]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try

    End Sub

    Private Sub BTSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTSave.Click
        Try

            Dim _Channels As IList(Of Channel) = Channel.ListAll

            For i As Integer = 0 To _Channels.Count - 1
                If _Channels(i).IsTv = True Then
                    _Channels(i).GrabEpg = False
                    _Channels(i).Persist()
                End If
            Next

            For i As Integer = 0 To EPGgrabData.Rows.Count - 1
                If CBool(EPGgrabData.Rows(i).Cells(0).Value) = True Then

                    Dim _channel As Channel = Channel.Retrieve(CInt(EPGgrabData.Rows(i).Cells(1).Value))

                    _channel.GrabEpg = True
                    _channel.Persist()
                End If
            Next
            Me.Close()

            MsgBox("Saving succesful ...", MsgBoxStyle.Information)

        Catch ex As Exception
            Log.[Error]("TVMovie: [SaveEnableEPGgrabbing]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Sub MappingData_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles EPGgrabData.CellFormatting
        'Hintergrundfarbe MappingData ändern, sofern channelName übereinstimmen
        With EPGgrabData.Rows(e.RowIndex)
            If CBool(.Cells(3).Value) = True Then
                .DefaultCellStyle.BackColor = Color.FromArgb(206, 255, 206)
            Else
                .DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255)
            End If
        End With
    End Sub

    Private Sub CBCeck_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBCeck.CheckedChanged

        For i As Integer = 0 To EPGgrabData.Rows.Count - 1
            If CBCeck.CheckState = Windows.Forms.CheckState.Checked Then
                EPGgrabData.Rows(i).Cells(0).Value = True
            Else
                EPGgrabData.Rows(i).Cells(0).Value = False
            End If
        Next

    End Sub

    Private Sub BTRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BTRefresh.Click
        EPGgrabData.Visible = False
        FillEPGgrabData()
        EPGgrabData.Visible = True
    End Sub
End Class