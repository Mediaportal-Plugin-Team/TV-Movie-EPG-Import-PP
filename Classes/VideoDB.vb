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

Public Class VideoDB
    Implements IDisposable

#Region "Members"
    Private disposed As Boolean = False
    Private m_db As SQLiteClient = Nothing
    Private Shared _VideoDBInfos As SQLiteResultSet
    Private _VideoDBID As Integer
    Private Shared _Index As Integer
#End Region

#Region "Constructors"
    Public Sub New()
        OpenVideoDB()
    End Sub

    <MethodImpl(MethodImplOptions.Synchronized)> _
    Private Sub OpenVideoDB()
        Try
            ' Maybe called by an exception
            If m_db IsNot Nothing Then
                Try
                    m_db.Close()
                    m_db.Dispose()
                    Log.Debug("TVMovie: [OpenVideoDB]: Disposing current instance..")
                Catch generatedExceptionName As Exception
                End Try
            End If


            ' Open database
            Dim layer As New TvBusinessLayer
            If File.Exists(layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\VideoDatabaseV5.db3") = True Then

                m_db = New SQLiteClient(layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\VideoDatabaseV5.db3")
                ' Retry 10 times on busy (DB in use or system resources exhausted)
                m_db.BusyRetries = 10
                ' Wait 100 ms between each try (default 10)
                m_db.BusyRetryDelay = 10

                DatabaseUtility.SetPragmas(m_db)
            Else
                Log.[Error]("TVMovie: [OpenVideoDB]: VideoDatabase not found: {0}", layer.GetSetting("TvMovieMPDatabase", "%ProgramData%\Team MediaPortal\MediaPortal\database").Value & "\VideoDatabaseV5.db3")
            End If


        Catch ex As Exception
            Log.[Error]("TVMovie: [OpenVideoDB]: VideoDatabase exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenVideoDB()
        End Try
        'Log.Info("picture database opened")
    End Sub

    Public Sub LoadAllVideoDBFilms()

        Try
            _VideoDBInfos = m_db.Execute("SELECT idMovie, strTitle, fRating FROM movieinfo ORDER BY strTitle ASC")
            Log.Info("TVMovie: [LoadAllVideoDBFilms]: success")


        Catch ex As Exception
            Log.[Error]("TVMovie: [LoadAllVideoDBFilms]: exception err:{0} stack:{1}", ex.Message, ex.StackTrace)
            OpenVideoDB()
        End Try

    End Sub
#End Region

#Region "Properties"
    Public ReadOnly Property Count() As Integer
        Get
            If _VideoDBInfos IsNot Nothing AndAlso _VideoDBInfos.Rows.Count > 0 Then
                Return _VideoDBInfos.Rows.Count
            Else
                Return 0
            End If
        End Get
    End Property

    'Get DBFields over Index
    Private _Item As New VideoDBItem
    Default Public ReadOnly Property VideoDatabase(ByVal Index As Integer) As VideoDBItem
        Get
            _Index = Index
            Return _Item
        End Get
    End Property
    Public Class VideoDBItem
        Public ReadOnly Property VideoID() As Integer
            Get
                If _VideoDBInfos IsNot Nothing AndAlso _VideoDBInfos.Rows.Count > 0 Then
                    Return CInt(DatabaseUtility.[Get](_VideoDBInfos, _Index, "idMovie"))
                Else
                    Return 0
                End If
            End Get
        End Property
        Public ReadOnly Property Title() As String
            Get
                If _VideoDBInfos IsNot Nothing AndAlso _VideoDBInfos.Rows.Count > 0 Then
                    Return DatabaseUtility.[Get](_VideoDBInfos, _Index, "strTitle")
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property Rating() As Integer
            Get
                If _VideoDBInfos IsNot Nothing AndAlso _VideoDBInfos.Rows.Count > 0 Then
                    Return CInt(Replace(DatabaseUtility.[Get](_VideoDBInfos, 0, "fRating"), ".", ","))
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
