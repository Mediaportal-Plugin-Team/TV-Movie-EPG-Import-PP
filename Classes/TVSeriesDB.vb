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
    Private disposed As Boolean = False
    Private m_db As SQLiteClient = Nothing
    Private _EpisodeInfos As SQLiteResultSet
    Private Shared _SeriesInfos As SQLiteResultSet
    Private _TVSeriesThumbPath As String
    Private _TVSeriesFanArtPath As String
    Private _SeriesID As Integer
    Private _EpisodeID As String
    Private Shared _Index As Integer


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
                    Log.Debug("TVMovie: [OpenTvSeriesDB]: Disposing current instance..")
                Catch generatedExceptionName As Exception
                End Try
            End If


            ' Open database
            Dim layer As New TvBusinessLayer
            If File.Exists(layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\TVSeriesDatabase4.db3") = True Then

                m_db = New SQLiteClient(layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\TVSeriesDatabase4.db3")
                ' Retry 10 times on busy (DB in use or system resources exhausted)
                m_db.BusyRetries = 10
                ' Wait 100 ms between each try (default 10)
                m_db.BusyRetryDelay = 10

                DatabaseUtility.SetPragmas(m_db)
            Else
                Log.[Error]("TVMovie: [OpenTvSeriesDB]: TvSeries Database not found: {0}", layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\TVSeriesDatabase4.db3")
            End If


        Catch ex As Exception
            Log.[Error]("TVMovie: [OpenTvSeriesDB]: TvSeries Database exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            MsgBox(ex.Message)
            OpenTvSeriesDB()
        End Try
        'Log.Info("picture database opened")
    End Sub

    Public Sub LoadAllSeries()

        Try
            _SeriesInfos = m_db.Execute("SELECT ID, Pretty_Name, PosterBannerFileName, CurrentBannerFileName, Rating FROM online_series WHERE ID > 0 ORDER BY Pretty_Name ASC")
            Log.Info("TVMovie: [LoadAllSeries]: success")
        Catch ex As Exception
            Log.[Error]("TVMovie: [LoadAllSeries]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenTvSeriesDB()
        End Try

    End Sub

    Public Function EpisodeFound(ByVal SeriesID As Integer, ByVal EpisodeName As String) As Boolean

        EpisodeName = Replace(Replace(Replace(Replace(Replace(Replace(EpisodeName, "'", "''"), "!", "%"), ".", "%"), " ", "%"), ":", "%"), "?", "%")

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
            Log.[Error]("TVMovie: [EpisodeFound]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenTvSeriesDB()
        End Try

    End Function


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
                Log.[Error]("TVMovie: [EpisodeExistLocal]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
                OpenTvSeriesDB()
            End Try
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












































'#Region "Variablen"
'    Implements IDisposable
'    Private Shared _Results As SQLiteResultSet
'    Private Shared _AvailableSeries As SQLiteResultSet
'    Private Shared _Index As Integer
'    Private Shared _TVSeriesDBPath As String
'    Private Shared _TVSeriesThumbPath As String
'    Private Shared _TVSeriesFanArtPath As String
'    Private Shared _disposed As Boolean = False
'    Private _Epsiodefound As Boolean

'    Private m_db As SQLiteClient = Nothing

'#End Region

'    Public Function _EpisodeFound() As Boolean
'        Return _Epsiodefound
'    End Function
'    Public Sub New(ByVal SeriesName As String, ByVal EpisodeName As String)
'        Try
'            If m_db IsNot Nothing Then
'                Try
'                    m_db.Close()
'                    m_db.Dispose()
'                    Log.Warn("Clickfinder ProgramGuide: [OpenTvSeriesDatabaseConnection]: Disposing current instance..")
'                Catch generatedExceptionName As Exception
'                End Try
'            End If

'            Dim strSQL As String = [String].Format("SELECT Count (ID) FROM online_series WHERE Pretty_Name LIKE '{0}'", SeriesName)

'            _Results = m_db.Execute(strSQL)

'            If _Results IsNot Nothing Then
'                If DatabaseUtility.GetAsInt(_Results, 0, 0) > 0 Then
'                    'MsgBox("Jup")
'                End If
'            End If



'            '            Dim m_db As SQLiteClient = Nothing
'            '            m_db = New SQLiteClient(Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3"))
'            '            m_db.BusyRetries = 10
'            '            m_db.BusyRetryDelay = 10

'            '            Dim strSQL As String = [String].Format("SELECT * FROM online_episodes INNER JOIN online_series ON online_episodes.SeriesID = online_series.ID WHERE Pretty_Name LIKE '{0}' AND EpisodeName LIKE '{1}'", _
'            '            SeriesName, EpisodeName)


'            'Try

'            '    For i As Integer = 0 To _AvailableSeries.Rows.Count - 1

'            '        _Epsiodefound = True

'            '        Dim _coloumnIndex As Integer = _AvailableSeries.ColumnIndices("Pretty_Name")
'            '        Dim _EpisodeName As Integer = _AvailableSeries.ColumnIndices("EpisodeName")
'            '        If SeriesName = _AvailableSeries.Rows(i).fields(_coloumnIndex) Then
'            '            Dim m_db As SQLiteClient = Nothing
'            '            m_db = New SQLiteClient(Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3"))
'            '            m_db.BusyRetries = 10
'            '            m_db.BusyRetryDelay = 10

'            '            Dim strSQL As String = [String].Format("SELECT * FROM online_episodes INNER JOIN online_series ON online_episodes.SeriesID = online_series.ID WHERE Pretty_Name LIKE '{0}' AND EpisodeName LIKE '{1}'", _
'            '            SeriesName, EpisodeName)


'            '            _Results = m_db.Execute(strSQL)
'            '            m_db.Close()

'            '            MsgBox(_Epsiodefound)
'            '            'MsgBox(_Results.Rows(0).fields(_coloumnIndex) & _Results.Rows(0).fields(_EpisodeName))

'            '        Else
'            '            _Results = Nothing
'            '            _Epsiodefound = False
'            '        End If

'            '    Next

'            'Dim m_db As SQLiteClient = Nothing
'            'm_db = New SQLiteClient(Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3"))

'            ''Dim strSQL As String = [String].Format("SELECT * FROM online_episodes INNER JOIN online_series ON online_episodes.SeriesID = online_series.ID WHERE Pretty_Name LIKE '{0}' AND EpisodeName LIKE '{1}'", _
'            ''SeriesName, EpisodeName)

'            'm_db.BusyRetries = 10
'            'm_db.BusyRetryDelay = 10

'            'Dim strSQL As String = [String].Format("SELECT * FROM online_series WHERE Pretty_Name LIKE '{0}'", _
'            'SeriesName)

'            '_Results = m_db.Execute(strSQL)

'            'Dim DataAdapter As SQLiteDataAdapter
'            'Dim Data As New DataSet
'            ''Dim Con As New SQLiteConnection
'            'Dim Cmd As New SQLiteCommand

'            '_TVSeriesDBPath = Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3")
'            '_TVSeriesThumbPath = Config.GetFile(Config.Dir.Thumbs, "MPTVSeriesBanners")
'            '_TVSeriesFanArtPath = Config.GetFile(Config.Dir.Thumbs, "Fan Art\")
'            ''Config.GetFile(Config.Dir.Thumbs, "MPTVSeriesBanners")
'            'Dim SQLString As String = "SELECT * FROM online_episodes INNER JOIN online_series ON online_episodes.SeriesID = online_series.ID WHERE Pretty_Name LIKE '" & Replace(SeriesName, "-", "%") & "' AND EpisodeName LIKE '" & EpisodeName & "'"

'            'Try

'            '    Con.ConnectionString = "Data Source=" & "\\MEDIASTATION\database\TVSeriesDatabase4.db3"
'            '    Con.Open()

'            '    DataAdapter = New SQLiteDataAdapter(SQLString, Con)
'            '    DataAdapter.Fill(Data, "TVSeriesDB")

'            '    DataAdapter.Dispose()
'            '    Con.Close()

'            '    _Table = Data.Tables("TVSeriesDB")

'        Catch ex As Exception
'            MsgBox(ex.Message)
'        End Try

'    End Sub
'    'Public Shared Sub LoadAvailableSeries()
'    '    Dim m_db As SQLiteClient = Nothing
'    '    m_db = New SQLiteClient(Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3"))
'    '    m_db.BusyRetries = 10
'    '    m_db.BusyRetryDelay = 10
'    '    Dim strSQL As String = [String].Format("SELECT * FROM online_series")
'    '    _AvailableSeries = m_db.Execute(strSQL)
'    '    m_db.Close()
'    'End Sub
'    Public Sub OpenTvSeriesDatabaseConnection()
'        ' Maybe called by an exception
'        If m_db IsNot Nothing Then
'            Try
'                m_db.Close()
'                m_db.Dispose()
'                Log.Warn("Clickfinder ProgramGuide: [OpenTvSeriesDatabaseConnection]: Disposing current instance..")
'            Catch generatedExceptionName As Exception
'            End Try

'            ' Open database
'            m_db = New SQLiteClient(Config.GetFile(Config.Dir.Database, "TVSeriesDatabase4.db3"))
'            m_db.BusyRetries = 10
'            m_db.BusyRetryDelay = 10

'            DatabaseUtility.SetPragmas(m_db)

'            Try

'            Catch ex As Exception
'                MsgBox(ex.Message)
'            End Try
'        End If
'    End Sub

'    Public Sub Dispose() Implements IDisposable.Dispose
'        If Not _disposed Then
'            _disposed = True
'            If m_db IsNot Nothing Then
'                Try
'                    m_db.Close()
'                    m_db.Dispose()
'                Catch generatedExceptionName As Exception
'                End Try
'                m_db = Nothing
'            End If
'        End If
'    End Sub


'#Region "Properties"
'    Public ReadOnly Property Count() As Integer
'        Get
'            If Not _Results.Rows.Count = Nothing Then
'                Return _Results.Rows.Count
'            Else
'                Return 0
'            End If

'        End Get
'    End Property
'    Public ReadOnly Property EpisodeFound() As Boolean
'        Get
'            Return _Epsiodefound
'        End Get
'    End Property

'    'Get DBFields over Index
'    Private _Item As New DataBaseItem
'    Default Public ReadOnly Property Item(ByVal Index As Integer) As DataBaseItem
'        Get
'            _Index = Index
'            Return _Item
'        End Get
'    End Property



'        'Public ReadOnly Property CompositeID() As String
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("CompositeID")) Then
'        '            Return _Table.Rows(_Index).Item("CompositeID")
'        '        Else
'        '            Return ""
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property SeasonIndex() As Integer
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("SeasonIndex")) Then
'        '            Return _Table.Rows(_Index).Item("SeasonIndex")
'        '        Else
'        '            Return 0
'        '        End If
'        '    End Get
'        'End Property

'        'Public ReadOnly Property EpisodeIndex() As Integer
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("EpisodeIndex")) Then
'        '            Return _Table.Rows(_Index).Item("EpisodeIndex")
'        '        Else
'        '            Return 0
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpisodeDescribtion() As String
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("Summary")) Then
'        '            Return _Table.Rows(_Index).Item("Summary")
'        '        Else
'        '            Return ""
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpisodeRating() As Double
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("Rating")) Then
'        '            Return Replace(_Table.Rows(_Index).Item("Rating"), ".", ",")
'        '        Else
'        '            Return 0
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpisodeRatingCount() As Double
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("RatingCount")) Then
'        '            Return Replace(_Table.Rows(_Index).Item("RatingCount"), ".", ",")
'        '        Else
'        '            Return 0
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpisodeFirstAired() As Date
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("FirstAired")) Then
'        '            Return _Table.Rows(_Index).Item("FirstAired")
'        '        Else
'        '            Return Nothing
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpisodeDirector() As String
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("Director")) Then
'        '            Return _Table.Rows(_Index).Item("Director")
'        '        Else
'        '            Return ""
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpisodeWriter() As String
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("Writer")) Then
'        '            Return _Table.Rows(_Index).Item("Writer")
'        '        Else
'        '            Return ""
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpisodeGuestStars() As String
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("GuestStars")) Then
'        '            Return _Table.Rows(_Index).Item("GuestStars")
'        '        Else
'        '            Return ""
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpisodeThumbFilename() As String
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("thumbFilename")) Then
'        '            Return _TVSeriesThumbPath & "\" & _Table.Rows(_Index).Item("thumbFilename")
'        '        Else
'        '            Return ""
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property SeriesCurrentBannerFileName() As String
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("CurrentBannerFileName")) Then
'        '            Return _TVSeriesThumbPath & _Table.Rows(_Index).Item("CurrentBannerFileName")
'        '        Else
'        '            Return ""
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property SeriesPosterBannerFileName() As String
'        '    Get
'        '        If Not IsDBNull(_Table.Rows(_Index).Item("PosterBannerFileName")) Then
'        '            Return _TVSeriesThumbPath & _Table.Rows(_Index).Item("PosterBannerFileName")
'        '        Else
'        '            Return ""
'        '        End If
'        '    End Get
'        'End Property
'        'Public ReadOnly Property SeriesFanArtFileName() As String
'        '    Get
'        '        Dim Con As New SQLiteConnection
'        '        Dim Cmd As New SQLiteCommand
'        '        Dim Data As SQLiteDataReader
'        '        Dim Path As String = ""
'        '        Dim SQLString As String = "SELECT * FROM Fanart WHERE SeriesID = '" & _Table.Rows(_Index).Item("seriesID") & "' ORDER BY Rating DESC"

'        '        Try

'        '            Con.ConnectionString = "Data Source=" & _TVSeriesDBPath
'        '            Con.Open()

'        '            Cmd = Con.CreateCommand
'        '            Cmd.CommandText = SQLString

'        '            Data = Cmd.ExecuteReader

'        '            While Data.Read

'        '                If Not Data.Item("LocalPath") = "" Then
'        '                    Path = _TVSeriesFanArtPath & Data.Item("LocalPath")
'        '                    Exit While
'        '                End If

'        '            End While

'        '            Cmd.Dispose()
'        '            Con.Close()

'        '            Return Path

'        '        Catch ex As Exception
'        '            MsgBox(ex.Message)
'        '            Return ""
'        '            Cmd.Dispose()
'        '            Con.Close()
'        '        End Try
'        '    End Get
'        'End Property
'        'Public ReadOnly Property EpsiodeExistLocal() As Boolean
'        '    Get
'        '        Dim Con As New SQLiteConnection
'        '        Dim Cmd As New SQLiteCommand
'        '        Dim Count As Long
'        '        Dim SQLString As String = "SELECT Count (*) As Anzahl FROM local_episodes WHERE CompositeID = '" & _Table.Rows(_Index).Item("CompositeID") & "'"

'        '        Try

'        '            Con.ConnectionString = "Data Source=" & _TVSeriesDBPath
'        '            Con.Open()

'        '            Cmd = Con.CreateCommand
'        '            Cmd.CommandText = SQLString

'        '            Count = Cmd.ExecuteScalar

'        '            Cmd.Dispose()
'        '            Con.Close()

'        '        Catch ex As Exception
'        '            MsgBox(ex.Message)
'        '            Cmd.Dispose()
'        '            Con.Close()
'        '        End Try

'        '        If Count > 0 Then
'        '            Return True
'        '        Else
'        '            Return False
'        '        End If

'        '    End Get
'        'End Property

'    End Class
'#End Region


