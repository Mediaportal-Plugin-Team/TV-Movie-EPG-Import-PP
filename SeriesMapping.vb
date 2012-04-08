Imports TvMovie.TvDatabase

Public Class frmSeriesMapping

    Private Sub frmSeriesMapping_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Alle Serien aus DB laden
        Dim _TvSeriesDB As New TVSeriesDB
        _TvSeriesDB.LoadAllSeries()

        'Alle Serien + idSeries in CB schreiben
        For i As Integer = 0 To _TvSeriesDB.CountSeries - 1
            CBSeries.Items.Add(_TvSeriesDB(i).SeriesID & " | " & _TvSeriesDB(i).SeriesName)
        Next

    End Sub

    Private Sub CBSeries_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CBSeries.SelectedIndexChanged
        Try
            Dim _SeriesMapping As TvMovieSeriesMapping = TvMovieSeriesMapping.Retrieve(CInt(Strings.Left(CBSeries.Text, InStr(CBSeries.Text, "|") - 2)))
            tbEpgName.Text = _SeriesMapping.EpgTitle
        Catch ex As Exception
            tbEpgName.Text = ""
        End Try
    End Sub

    Private Sub ButtonSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonSave.Click
        If Not String.IsNullOrEmpty(CBSeries.Text) Then

            If Not String.IsNullOrEmpty(tbEpgName.Text) Then
                Try
                    Dim _SeriesMapping As TvMovieSeriesMapping = TvMovieSeriesMapping.Retrieve(CInt(Strings.Left(CBSeries.Text, InStr(CBSeries.Text, "|") - 2)))
                    _SeriesMapping.EpgTitle = tbEpgName.Text
                    _SeriesMapping.Persist()
                Catch ex As Exception
                    Dim _SeriesMapping As New TvMovieSeriesMapping(CInt(Strings.Left(CBSeries.Text, InStr(CBSeries.Text, "|") - 2)))
                    _SeriesMapping.EpgTitle = tbEpgName.Text
                    _SeriesMapping.Persist()
                End Try

            Else
                Try
                    Dim _SeriesMapping As TvMovieSeriesMapping = TvMovieSeriesMapping.Retrieve(CInt(Strings.Left(CBSeries.Text, InStr(CBSeries.Text, "|") - 2)))
                    _SeriesMapping.Remove()
                Catch ex As Exception
                End Try
            End If

        End If
    End Sub
End Class