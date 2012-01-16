Imports System.Reflection
Imports System.Data.OleDb
Imports MediaPortal.UserInterface.Controls
Imports MediaPortal.Configuration
Imports MediaPortal.Common
Imports Gentle.Common
Imports Gentle.Framework
Imports TvDatabase
Imports System.Data
Imports System.Collections


Public Class ClickfinderDB
#Region "Variablen"
    Private Shared _Table As DataTable
    Private Shared _TvServerTable As DataTable
    Private Shared _Index As Integer
    Private Shared _IndexColumn As Integer
    Private Shared _ClickfinderDataBaseFolder As String

#End Region

#Region "Properties"


    Public ReadOnly Property Count() As Integer
        Get
            Return _Table.Rows.Count
        End Get
    End Property
    Public ReadOnly Property DatensatzTabelle() As DataTable
        Get
            Return _Table
        End Get
    End Property

    'Get DBFields over Index
    Private _Item As New DataBaseItem
    Default Public ReadOnly Property Item(ByVal Index As Integer) As DataBaseItem
        Get
            _Index = Index
            Return _Item
        End Get
    End Property
    Public Class DataBaseItem





        'SubProperties for _item





        Public ReadOnly Property Titel() As String
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("Titel")) Then
                    Return CStr(_Table.Rows(_Index).Item("Titel"))
                Else
                    Return ""
                End If
            End Get
        End Property

        Public ReadOnly Property Beginn() As Date
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("Beginn")) Then
                    Return CDate(_Table.Rows(_Index).Item("Beginn"))
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Public ReadOnly Property Ende() As Date
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("Ende")) Then
                    Return CDate(_Table.Rows(_Index).Item("Ende"))
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly Property SenderKennung() As String
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("SenderKennung")) Then
                    Return CStr(_Table.Rows(_Index).Item("SenderKennung"))
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property Bewertung() As Integer
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("Bewertung")) Then
                    Return CInt(_Table.Rows(_Index).Item("Bewertung"))
                Else
                    Return 0
                End If
            End Get
        End Property
        Public ReadOnly Property KzLive() As Boolean
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("KzLive")) Then
                    Return CBool(_Table.Rows(_Index).Item("KzLive"))
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property KzWiederholung() As Boolean
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("KzWiederholung")) Then
                    Return CBool(_Table.Rows(_Index).Item("KzWiederholung"))
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property Darsteller() As String
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("Darsteller")) Then
                    Return Replace(CStr(_Table.Rows(_Index).Item("Darsteller")), ";", " ")
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property Regie() As String
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("Regie")) Then
                    Return Replace(CStr(_Table.Rows(_Index).Item("Regie")), ";", " ")
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property Herstellungsland() As String
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("Herstellungsland")) Then
                    Return Replace(CStr(_Table.Rows(_Index).Item("Herstellungsland")), ";", " ")
                Else
                    Return ""
                End If
            End Get
        End Property
        Public ReadOnly Property Herstellungsjahr() As String
            Get
                If Not IsDBNull(_Table.Rows(_Index).Item("Herstellungsjahr")) Then
                    Return Replace(CStr(_Table.Rows(_Index).Item("Herstellungsjahr")), ";", " ")
                Else
                    Return ""
                End If
            End Get
        End Property

    End Class

    Public ReadOnly Property DataTable() As DataTable
        Get
            Return _Table
        End Get
    End Property

#End Region

    Public Sub New(ByVal SQLString As String, ByVal DatabasePath As String)
        'Fill Dataset with SQLString Query
        Dim DataAdapter As OleDb.OleDbDataAdapter
        Dim Data As New DataSet

        Dim layer As New TvBusinessLayer()

        _ClickfinderDataBaseFolder = System.IO.Path.GetDirectoryName(DatabasePath)

        Try

            Dim Con As New OleDb.OleDbConnection
            Dim Cmd As New OleDb.OleDbCommand

            Con.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" & "Data Source=" & DatabasePath
            Con.Open()

            DataAdapter = New OleDbDataAdapter(SQLString, Con)
            DataAdapter.Fill(Data, "ClickfinderDB")

            DataAdapter.Dispose()
            Con.Close()

            _Table = Data.Tables("ClickfinderDB")

            'PrimärSchlüssel für Datatable festlegen, damit gesucht werden kann.
            _Table.PrimaryKey = New DataColumn() {_Table.Columns("Sendungen.Pos")}

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

End Class
