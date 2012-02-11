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

Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports TvLibrary.Log


Imports MediaPortal.Database
Imports SQLite.NET
Imports TvDatabase

Public Class TVSeriesDB

    Implements IDisposable

#Region "Member"
    Private disposed As Boolean = False
    Private Shared m_db As SQLiteClient = Nothing
    Private _EpisodeInfos As SQLiteResultSet
    Private Shared _SeriesInfos As SQLiteResultSet
    Private _TVSeriesThumbPath As String
    Private _TVSeriesFanArtPath As String
    Private _SeriesID As Integer
    Private _EpisodeID As String
    Private Shared _Index As Integer

#End Region

#Region "Constructors"
    Public Sub New()
        OpenTvSeriesDB()
    End Sub

    <MethodImpl(MethodImplOptions.Synchronized)> _
    Private Sub OpenTvSeriesDB()
        Try
            ' Maybe called by an exception
            If m_db IsNot Nothing Then
                Try
                    m_db.Close()
                    m_db.Dispose()
                    MyLog.Debug("TVMovie: [OpenTvSeriesDB]: Disposing current instance..")
                Catch generatedExceptionName As Exception
                End Try
            End If


            ' Open database
            Dim layer As New TvBusinessLayer
            If File.Exists(layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\TVSeriesDatabase4.db3") = True Then

                m_db = New SQLiteClient(layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\TVSeriesDatabase4.db3")
                ' Retry 10 times on busy (DB in use or system resources exhausted)
                m_db.BusyRetries = 20
                ' Wait 100 ms between each try (default 10)
                m_db.BusyRetryDelay = 1000

                DatabaseUtility.SetPragmas(m_db)
            Else
                MyLog.[Error]("TVMovie: [OpenTvSeriesDB]: TvSeries Database not found: {0}", layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\TVSeriesDatabase4.db3")
            End If


        Catch ex As Exception
            MyLog.[Error]("TVMovie: [OpenTvSeriesDB]: TvSeries Database exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenTvSeriesDB()
        End Try
        'Mylog.Info("picture database opened")
    End Sub

    Public Sub LoadAllSeries()

        Try
            _SeriesInfos = m_db.Execute("SELECT * FROM online_series WHERE ID > 0 ORDER BY Pretty_Name ASC")
            MyLog.Info("TVMovie: [LoadAllSeries]: success - {0} Series found", _SeriesInfos.Rows.Count)
        Catch ex As Exception
            MyLog.[Error]("TVMovie: [LoadAllSeries]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenTvSeriesDB()
        End Try

    End Sub

    Public Sub LoadEpisode(ByVal serieName As String, ByVal seriesNum As Integer, ByVal episodeNum As Integer)

        Try

            _SeriesInfos = m_db.Execute( _
                                [String].Format("SELECT * FROM online_series WHERE Pretty_Name LIKE '{0}' OR SortName LIKE '{0}' OR origName LIKE '{0}'", _
                                serieName))

            _EpisodeInfos = m_db.Execute( _
                                [String].Format("SELECT * FROM online_episodes WHERE SeriesID = '{0}' AND SeasonIndex = '{1}' AND EpisodeName = '{2}'", _
                                                Me.Series(0).SeriesID, seriesNum, episodeNum))

        Catch ex As Exception
            MyLog.[Error]("TVMovie: [LoadEpisode]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenTvSeriesDB()
        End Try

    End Sub

    Public Function EpisodeFound(ByVal SeriesID As Integer, ByVal EpisodeName As String) As Boolean

        Try
            _EpisodeInfos = m_db.Execute( _
                            [String].Format("SELECT * FROM online_episodes WHERE SeriesID = '{0}' AND EpisodeName LIKE '{1}'", _
                            SeriesID, EpisodeName))

            If _EpisodeInfos IsNot Nothing AndAlso _EpisodeInfos.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            MyLog.[Error]("TVMovie: [EpisodeFound]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenTvSeriesDB()
        End Try

    End Function
#End Region

#Region "Properties"
    Public ReadOnly Property CountSeries() As Integer
        Get
            If _SeriesInfos IsNot Nothing AndAlso _SeriesInfos.Rows.Count > 0 Then
                Return _SeriesInfos.Rows.Count
            Else
                Return 0
            End If
        End Get
    End Property

    'Get DBFields over Index
    Private _Item As New SeriesItem
    Default Public ReadOnly Property Series(ByVal Index As Integer) As SeriesItem
        Get
            _Index = Index
            Return _Item
        End Get
    End Property
    Public Class SeriesItem
        Public ReadOnly Property SeriesID() As Integer
            Get
                If _SeriesInfos IsNot Nothing AndAlso _SeriesInfos.Rows.Count > 0 Then
                    Return CInt(DatabaseUtility.[Get](_SeriesInfos, _Index, "ID"))
                Else
                    Return 0
                End If
            End Get
        End Property
        Public ReadOnly Property SeriesName() As String
            Get
                If _SeriesInfos IsNot Nothing AndAlso _SeriesInfos.Rows.Count > 0 Then
                    Return DatabaseUtility.[Get](_SeriesInfos, _Index, "Pretty_Name")
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property SeriesorigName() As String
            Get
                If _SeriesInfos IsNot Nothing AndAlso _SeriesInfos.Rows.Count > 0 Then
                    Return DatabaseUtility.[Get](_SeriesInfos, _Index, "origName")
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property SeriesPosterImage() As String
            Get
                If _SeriesInfos IsNot Nothing AndAlso _SeriesInfos.Rows.Count > 0 Then
                    Return DatabaseUtility.[Get](_SeriesInfos, _Index, "PosterBannerFileName")
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property FanArt() As String
            Get
                Dim _result As SQLiteResultSet
                Dim strSQL As String = [String].Format("SELECT * FROM Fanart WHERE seriesID = '{0}' AND LocalPath LIKE '_%'", SeriesID)

                _result = m_db.Execute(strSQL)

                If _result IsNot Nothing AndAlso _result.Rows.Count > 0 Then
                    For i As Integer = 0 To _result.Rows.Count - 1

                        If Not String.IsNullOrEmpty(DatabaseUtility.[Get](_result, i, "LocalPath")) Then
                            Return DatabaseUtility.[Get](_result, i, "LocalPath")
                            Exit Property
                        End If
                    Next
                    Return ""
                Else
                    Return ""
                End If

            End Get
        End Property
    End Class


    Public ReadOnly Property SeasonIndex() As Integer
        Get
            If _EpisodeInfos IsNot Nothing AndAlso _EpisodeInfos.Rows.Count > 0 Then
                Return CInt(DatabaseUtility.[Get](_EpisodeInfos, 0, "SeasonIndex"))
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property EpisodeIndex() As Integer
        Get
            If _EpisodeInfos IsNot Nothing AndAlso _EpisodeInfos.Rows.Count > 0 Then
                Return CInt(DatabaseUtility.[Get](_EpisodeInfos, 0, "EpisodeIndex"))
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property EpisodeRating() As Integer
        Get
            If _EpisodeInfos IsNot Nothing AndAlso _EpisodeInfos.Rows.Count > 0 Then
                Return CInt(Replace(DatabaseUtility.[Get](_EpisodeInfos, 0, "Rating"), ".", ","))
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property EpisodeCompositeID() As String
        Get
            If _EpisodeInfos IsNot Nothing AndAlso _EpisodeInfos.Rows.Count > 0 Then
                Return DatabaseUtility.[Get](_EpisodeInfos, 0, "CompositeID")
            Else
                Return ""
            End If
        End Get
    End Property
    Public ReadOnly Property EpisodeExistLocal() As Boolean
        Get
            Try
                Dim _result As SQLiteResultSet
                Dim strSQL As String = [String].Format("SELECT Count (CompositeID) FROM local_episodes WHERE CompositeID = '{0}'", EpisodeCompositeID)

                _result = m_db.Execute(strSQL)

                If _result IsNot Nothing Then
                    If DatabaseUtility.GetAsInt(_result, 0, 0) > 0 Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            Catch ex As Exception
                MyLog.[Error]("TVMovie: [EpisodeExistLocal]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                OpenTvSeriesDB()
            End Try
        End Get

    End Property
    Public ReadOnly Property EpisodeImage() As String
        Get
            If _EpisodeInfos IsNot Nothing AndAlso _EpisodeInfos.Rows.Count > 0 Then
                Return DatabaseUtility.[Get](_EpisodeInfos, 0, "thumbFilename")
            Else
                Return ""
            End If
        End Get
    End Property

#End Region

#Region "IDisposable Members"

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not disposed Then
            disposed = True
            If m_db IsNot Nothing Then
                Try
                    m_db.Close()
                    m_db.Dispose()
                Catch generatedExceptionName As Exception
                End Try
                m_db = Nothing
            End If
        End If
    End Sub
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

#End Region

End Class