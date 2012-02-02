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

Public Class MovingPicturesDB
    Implements IDisposable

#Region "Members"
    Private disposed As Boolean = False
    Private m_db As SQLiteClient = Nothing
    Private Shared _MovingPicturesInfos As SQLiteResultSet
    Private _MovingPicturesID As Integer
    Private Shared _Index As Integer
#End Region

#Region "Constructors"
    Public Sub New()
        OpenMovingPicturesDB()
    End Sub

    <MethodImpl(MethodImplOptions.Synchronized)> _
    Private Sub OpenMovingPicturesDB()
        Try
            ' Maybe called by an exception
            If m_db IsNot Nothing Then
                Try
                    m_db.Close()
                    m_db.Dispose()
                    MyLog.Debug("TVMovie: [OpenMovingPicturesDB]: Disposing current instance..")
                Catch generatedExceptionName As Exception
                End Try
            End If


            ' Open database
            Dim layer As New TvBusinessLayer
            If File.Exists(layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\movingpictures.db3") = True Then

                m_db = New SQLiteClient(layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\movingpictures.db3")
                ' Retry 10 times on busy (DB in use or system resources exhausted)
                m_db.BusyRetries = 20
                ' Wait 100 ms between each try (default 10)
                m_db.BusyRetryDelay = 1000

                DatabaseUtility.SetPragmas(m_db)
            Else
                MyLog.[Error]("TVMovie: [OpenMovingPicturesDB]: TvSeries Database not found: {0}", layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\movingpictures.db3")
            End If


        Catch ex As Exception
            MyLog.[Error]("TVMovie: [OpenMovingPicturesDB]: TvSeries Database exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenMovingPicturesDB()
        End Try
        'Mylog.Info("picture database opened")
    End Sub

    Public Sub LoadAllMovingPicturesFilms()

        Try
            _MovingPicturesInfos = m_db.Execute("SELECT id, title, alternate_titles, score FROM movie_info ORDER BY title ASC")
            MyLog.Info("TVMovie: [LoadAllMovingPicturesFilms]: success")
        Catch ex As Exception
            MyLog.[Error]("TVMovie: [LoadAllMovingPicturesFilms]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenMovingPicturesDB()
        End Try

    End Sub
#End Region

#Region "Properties"
    Public ReadOnly Property Count() As Integer
        Get
            If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                Return _MovingPicturesInfos.Rows.Count
            Else
                Return 0
            End If
        End Get
    End Property

    'Get DBFields over Index
    Private _Item As New MovingPicturesItem
    Default Public ReadOnly Property MovingPictures(ByVal Index As Integer) As MovingPicturesItem
        Get
            _Index = Index
            Return _Item
        End Get
    End Property
    Public Class MovingPicturesItem
        Public ReadOnly Property MovingPicturesID() As Integer
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Return CInt(DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "id"))
                Else
                    Return 0
                End If
            End Get
        End Property
        Public ReadOnly Property Title() As String
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Return DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "title")
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property AlternateTitle() As String
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Return Replace(DatabaseUtility.[Get](_MovingPicturesInfos, _Index, "alternate_titles"), "|", "")
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property Rating() As Integer
            Get
                If _MovingPicturesInfos IsNot Nothing AndAlso _MovingPicturesInfos.Rows.Count > 0 Then
                    Return CInt(Replace(DatabaseUtility.[Get](_MovingPicturesInfos, 0, "score"), ".", ","))
                Else
                    Return 0
                End If
            End Get
        End Property
    End Class

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
