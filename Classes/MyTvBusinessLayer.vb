Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Gentle.Framework
Imports MySql.Data.MySqlClient
Imports TvDatabase
Imports TvLibrary
Imports TvLibrary.Channels
Imports TvLibrary.Implementations
Imports TvLibrary.Epg
Imports TvLibrary.Interfaces
Imports TvLibrary.Log
Imports StatementType = Gentle.Framework.StatementType
Imports ThreadState = System.Threading.ThreadState

Namespace TvEngine
    Public Class MyBusinessLayer
#Region "delegates"

        Private Delegate Sub InsertTvMovieProgramsDelegate(ByVal aImportParam As ImportParams)

#End Region
#Region "Members"
        Private Shared _insertProgramsThread As Thread = Nothing
        Private Shared _programInsertsQueue As New System.Collections.Queue
        Private Shared _pendingProgramInserts As New AutoResetEvent(False)
#End Region


        Public Function InsertTvMoviePrograms(ByVal aTvMprogramList As List(Of TvMprogram), ByVal aThreadPriority As ThreadPriority) As Integer
            Try
                Dim sleepTime As Integer = 10

                Select Case aThreadPriority
                    Case ThreadPriority.Highest, ThreadPriority.AboveNormal
                        aThreadPriority = ThreadPriority.Normal
                        sleepTime = 0
                        Exit Select
                    Case ThreadPriority.Normal
                        ' this is almost enough on dualcore systems for one cpu to gather epg and the other to insert it
                        sleepTime = 10
                        Exit Select
                    Case ThreadPriority.BelowNormal
                        ' on faster systems this might be enough for background importing
                        sleepTime = 20
                        Exit Select
                    Case ThreadPriority.Lowest
                        ' even a single core system is enough to use MP while importing.
                        sleepTime = 40
                        Exit Select
                End Select

                Dim param As New ImportParams()
                param.TvMprogramList = aTvMprogramList
                param.SleepTime = sleepTime
                param.Priority = aThreadPriority

                SyncLock _programInsertsQueue
                    _programInsertsQueue.Enqueue(param)
                    _pendingProgramInserts.[Set]()

                    If _insertProgramsThread Is Nothing Then
                        _insertProgramsThread = New Thread(AddressOf InsertProgramsThreadStart) With { _
                         .Priority = ThreadPriority.Lowest, _
                         .Name = "SQL EPG importer", _
                         .IsBackground = True _
                        }
                        _insertProgramsThread.Start()
                    End If
                End SyncLock

                Return aTvMprogramList.Count
            Catch ex As Exception
                MyLog.[Error]("MyBusinessLayer: InsertPrograms error - {0}, {1}", ex.Message, ex.StackTrace)
                Return 0
            End Try
        End Function

        Private Class ImportParams
            Public TvMprogramList As List(Of TvMprogram)
            Public ConnectString As String
            Public Priority As ThreadPriority
            Public SleepTime As Integer
        End Class

        Private Shared Sub InsertProgramsThreadStart()
            Try
                'MyLog.Debug("MyBusinessLayer: InsertProgramsThread started")

                Dim prov As IGentleProvider = ProviderFactory.GetDefaultProvider()
                Dim provider As String = prov.Name.ToLowerInvariant()
                Dim defaultConnectString As String = prov.ConnectionString
                Dim lastImport As DateTime = DateTime.Now
                Dim insertProgams As InsertTvMovieProgramsDelegate

                Select Case provider
                    Case "mysql"
                        insertProgams = AddressOf InsertProgramsMySql
                        Exit Select
                    Case "sqlserver"
                        insertProgams = AddressOf InsertProgramsSqlServer
                        Exit Select
                    Case Else
                        MyLog.Info("MyBusinessLayer: InsertPrograms unknown provider - {0}", provider)
                        Return
                End Select

                While True
                    If lastImport.AddSeconds(60) < DateTime.Now Then
                        ' Done importing and 60 seconds since last import
                        SyncLock _programInsertsQueue
                            '  Has new work been queued in the meantime?
                            If _programInsertsQueue.Count = 0 Then
                                'MyLog.Debug("MyBusinessLayer: InsertProgramsThread exiting")
                                _insertProgramsThread = Nothing
                                Exit While
                            End If
                        End SyncLock
                    End If

                    _pendingProgramInserts.WaitOne(10000)
                    ' Check every 10 secs
                    While _programInsertsQueue.Count > 0
                        Try
                            Dim importParams As ImportParams
                            SyncLock _programInsertsQueue
                                importParams = _programInsertsQueue.Dequeue()
                            End SyncLock
                            importParams.ConnectString = defaultConnectString
                            Thread.CurrentThread.Priority = importParams.Priority
                            insertProgams(importParams)
                            MyLog.Info("MyBusinessLayer: Inserted {0} programs to the database", importParams.TvMprogramList.Count)
                            lastImport = DateTime.Now
                            Thread.CurrentThread.Priority = ThreadPriority.Lowest
                        Catch ex As Exception
                            MyLog.[Error]("MyBusinessLayer: InsertMySQL/InsertMSSQL caused an exception:")
                            MyLog.Write(ex)
                        End Try
                    End While
                    ' Now all queued inserts have been processed, clear Gentle cache
                    Gentle.Common.CacheManager.ClearQueryResultsByType(GetType(Program))
                End While
            Catch ex As Exception
                MyLog.[Error]("MyBusinessLayer: InsertProgramsThread error - {0}, {1}", ex.Message, ex.StackTrace)
            End Try
        End Sub

        Private Shared Sub InsertProgramsMySql(ByVal aImportParam As ImportParams)
            Dim transact As MySqlTransaction = Nothing
            Try
                Using connection As New MySqlConnection(aImportParam.ConnectString)

                    connection.Open()
                    transact = connection.BeginTransaction()

                    ExecuteInsertProgramsMySqlCommand(aImportParam.TvMprogramList, connection, transact, aImportParam.SleepTime)
                    'OptimizeMySql("Program");
                    transact.Commit()
                End Using
            Catch ex As Exception
                Try
                    If transact IsNot Nothing Then
                        transact.Rollback()
                    End If
                Catch ex2 As Exception
                    MyLog.Info("MyBusinessLayer: InsertSqlServer unsuccessful - ROLLBACK - {0}, {1}", ex2.Message, ex2.StackTrace)
                End Try
                MyLog.Info("MyBusinessLayer: InsertMySql caused an Exception - {0}, {1}", ex.Message, ex.StackTrace)
            End Try
        End Sub
        Private Shared Sub InsertProgramsSqlServer(ByVal aImportParam As ImportParams)

        End Sub

        Private Shared Sub ExecuteInsertProgramsMySqlCommand(ByVal aProgramList As IEnumerable(Of TvMprogram), ByVal aConnection As MySqlConnection, ByVal aTransaction As MySqlTransaction, ByVal aDelay As Integer)
            Dim aCounter As Integer = 0
            Dim sqlCmd As New MySqlCommand()
            Dim currentInserts As New List(Of TvMprogram)(aProgramList)

            sqlCmd.CommandText = "INSERT INTO mptvdb.test (idProgram, TvMovieBewertung, Kurzkritik, Bilddateiname, Fun, Action, Feelings, Erotic, Tension, Requirement, Actors, Dolby, HDTV, Country, Regie, Describtion, ShortDescribtion) VALUES (?idProgram, ?TvMovieBewertung, ?Kurzkritik, ?Bilddateiname, ?Fun, ?Action, ?Feelings, ?Erotic, ?Tension, ?Requirement, ?Actors, ?Dolby, ?HDTV, ?Country, ?Regie, ?Describtion, ?ShortDescribtion)"

            sqlCmd.Parameters.Add("?idProgram", MySqlDbType.Int32)
            sqlCmd.Parameters.Add("?TvMovieBewertung", MySqlDbType.Int32)
            sqlCmd.Parameters.Add("?Bilddateiname", MySqlDbType.VarChar)
            sqlCmd.Parameters.Add("?Kurzkritik", MySqlDbType.VarChar)
            sqlCmd.Parameters.Add("?Fun", MySqlDbType.Int32)
            sqlCmd.Parameters.Add("?Action", MySqlDbType.Int32)
            sqlCmd.Parameters.Add("?Feelings", MySqlDbType.Int32)
            sqlCmd.Parameters.Add("?Erotic", MySqlDbType.Int32)
            sqlCmd.Parameters.Add("?Tension", MySqlDbType.Int32)
            sqlCmd.Parameters.Add("?Requirement", MySqlDbType.Int32)
            sqlCmd.Parameters.Add("?Actors", MySqlDbType.Text)
            sqlCmd.Parameters.Add("?Dolby", MySqlDbType.Bit)
            sqlCmd.Parameters.Add("?HDTV", MySqlDbType.Bit)
            sqlCmd.Parameters.Add("?Country", MySqlDbType.VarChar)
            sqlCmd.Parameters.Add("?Regie", MySqlDbType.VarChar)
            sqlCmd.Parameters.Add("?Describtion", MySqlDbType.Text)
            sqlCmd.Parameters.Add("?ShortDescribtion", MySqlDbType.Text)

            Try
                sqlCmd.Connection = aConnection
                sqlCmd.Transaction = aTransaction
                ' Prepare the command since we will reuse it quite often
                sqlCmd.Prepare()
            Catch ex As Exception
                MyLog.Info("MyBusinessLayer: ExecuteInsertProgramsMySqlCommand - Prepare caused an Exception - {0}", ex.Message)
            End Try
            For Each prog As TvMprogram In currentInserts
                sqlCmd.Parameters("?idProgram").Value = prog.idProgram
                sqlCmd.Parameters("?TvMovieBewertung").Value = prog.Bewertung
                sqlCmd.Parameters("?Bilddateiname").Value = prog.Bilddateiname
                sqlCmd.Parameters("?Kurzkritik").Value = prog.Kurzkritik
                sqlCmd.Parameters("?Fun").Value = prog.Spass
                sqlCmd.Parameters("?Action").Value = prog.Action
                sqlCmd.Parameters("?Feelings").Value = prog.Gefuhl
                sqlCmd.Parameters("?Erotic").Value = prog.Erotik
                sqlCmd.Parameters("?Tension").Value = prog.Spannung
                sqlCmd.Parameters("?Requirement").Value = prog.Anspruch
                sqlCmd.Parameters("?Actors").Value = prog.Darsteller
                sqlCmd.Parameters("?Dolby").Value = prog.KzDolbyDigital
                sqlCmd.Parameters("?HDTV").Value = prog.KzHDTV
                sqlCmd.Parameters("?Country").Value = prog.Herstellungsland
                sqlCmd.Parameters("?Regie").Value = prog.Regie
                sqlCmd.Parameters("?Describtion").Value = prog.Beschreibung
                sqlCmd.Parameters("?ShortDescribtion").Value = prog.KurzBeschreibung

                Try
                    ' Finally insert all our data
                    sqlCmd.ExecuteNonQuery()
                    aCounter += 1
                    ' Avoid I/O starving
                    If aCounter Mod 3 = 0 Then
                        Thread.Sleep(aDelay)
                    End If
                Catch myex As MySqlException
                    Dim errorRow As String = sqlCmd.Parameters("?idProgram").Value + ", "
                    Select Case myex.Number
                        Case 1062
                            MyLog.Info("MyBusinessLayer: Your importer tried to add a duplicate entry: {0}", errorRow)
                            Exit Select
                        Case 1406
                            MyLog.Info("MyBusinessLayer: Your importer tried to add a too much info: {0}, {1}", errorRow, myex.Message)
                            Exit Select
                        Case Else
                            MyLog.Info("MyBusinessLayer: ExecuteInsertProgramsMySqlCommand caused a MySqlException - {0}, {1} {2}", myex.Message, myex.Number, myex.HelpLink)
                            Exit Select
                    End Select
                Catch ex As Exception
                    MyLog.Info("MyBusinessLayer: ExecuteInsertProgramsMySqlCommand caused an Exception - {0}, {1}", ex.Message, ex.StackTrace)
                End Try
            Next
            Return
        End Sub

        Public Sub WaitForInsertPrograms()
            Dim currentInsertThread As Thread = _insertProgramsThread
            If currentInsertThread IsNot Nothing AndAlso (currentInsertThread.ThreadState And ThreadState.Unstarted) <> ThreadState.Unstarted Then
                currentInsertThread.Join()
            End If
        End Sub

    End Class
End Namespace