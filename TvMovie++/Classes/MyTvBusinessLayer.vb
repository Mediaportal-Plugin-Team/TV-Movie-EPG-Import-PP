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

        Private Class ImportParams
            Public TvMprogramList As List(Of TvMprogram)
            Public ConnectString As String
            Public Priority As ThreadPriority
            Public SleepTime As Integer
        End Class

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
                        _insertProgramsThread = New Thread(AddressOf InsertTvMovieProgramsThreadStart) With { _
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
        Private Shared Sub InsertTvMovieProgramsThreadStart()
            Try
                'MyLog.Debug("MyBusinessLayer: InsertProgramsThread started")

                Dim prov As IGentleProvider = ProviderFactory.GetDefaultProvider()
                Dim provider As String = prov.Name.ToLowerInvariant()
                Dim defaultConnectString As String = prov.ConnectionString
                Dim lastImport As DateTime = DateTime.Now
                Dim insertProgams As InsertTvMovieProgramsDelegate

                Select Case provider
                    Case "mysql"
                        insertProgams = AddressOf InsertTvMovieProgramsMySql
                        MyLog.Info("MyBusinessLayer: provider - {0}", provider)
                        Exit Select
                    Case "sqlserver"
                        insertProgams = AddressOf InsertTvMovieProgramsSqlServer
                        MyLog.Info("MyBusinessLayer: provider - {0}", provider)
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
                            MyLog.Info("MyBusinessLayer: Inserted {0} TvMoviePrograms to the database", importParams.TvMprogramList.Count)
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
                MyLog.[Error]("MyBusinessLayer: InsertTvMovieProgramsThread error - {0}, {1}", ex.Message, ex.StackTrace)
            End Try
        End Sub

        Private Shared Sub InsertTvMovieProgramsMySql(ByVal aImportParam As ImportParams)
            Dim transact As MySqlTransaction = Nothing
            Try
                Using connection As New MySqlConnection(aImportParam.ConnectString)

                    connection.Open()
                    transact = connection.BeginTransaction()

                    ExecuteInsertTvMovieProgramsMySqlCommand(aImportParam.TvMprogramList, connection, transact, aImportParam.SleepTime)
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
        Private Shared Sub InsertTvMovieProgramsSqlServer(ByVal aImportParam As ImportParams)
            Dim transact As SqlTransaction = Nothing
            Try
                Using connection As New SqlConnection(aImportParam.ConnectString)

                    connection.Open()
                    transact = connection.BeginTransaction()

                    ExecuteInsertTvMovieProgramsSqlServerCommand(aImportParam.TvMprogramList, connection, transact, aImportParam.SleepTime)
                    transact.Commit()
                End Using
            Catch ex As Exception
                Try
                    If transact IsNot Nothing Then
                        transact.Rollback()
                    End If
                Catch ex2 As Exception
                    Log.Info("MyBusinessLayer: InsertSqlServer unsuccessful - ROLLBACK - {0}, {1}", ex2.Message, ex2.StackTrace)
                End Try
                Log.Info("MyBusinessLayer: InsertSqlServer caused an Exception - {0}, {1}", ex.Message, ex.StackTrace)
            End Try
        End Sub


        Private Shared Sub ExecuteInsertTvMovieProgramsMySqlCommand(ByVal aProgramList As IEnumerable(Of TvMprogram), ByVal aConnection As MySqlConnection, ByVal aTransaction As MySqlTransaction, ByVal aDelay As Integer)
            Dim aCounter As Integer = 0
            Dim sqlCmd As New MySqlCommand()
            Dim currentInserts As New List(Of TvMprogram)(aProgramList)

            sqlCmd.CommandText = "INSERT INTO mptvdb.TVMovieProgram (idProgram, TvMovieBewertung, Kurzkritik, Bilddateiname, Fun, Action, Feelings, Erotic, Tension, Requirement, Actors, Dolby, HDTV, Country, Regie, Describtion, ShortDescribtion) VALUES (?idProgram, ?TvMovieBewertung, ?Kurzkritik, ?Bilddateiname, ?Fun, ?Action, ?Feelings, ?Erotic, ?Tension, ?Requirement, ?Actors, ?Dolby, ?HDTV, ?Country, ?Regie, ?Describtion, ?ShortDescribtion)"

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
                    Dim errorRow As String = sqlCmd.Parameters("?idProgram").Value.ToString & ", " & sqlCmd.Parameters("?TvMovieBewertung").Value.ToString & ", " & sqlCmd.Parameters("?Kurzkritik").Value.ToString & ", " & sqlCmd.Parameters("?Bilddateiname").Value.ToString & ", " & sqlCmd.Parameters("?Fun").Value.ToString & ", " & sqlCmd.Parameters("?Action").Value.ToString & ", " & sqlCmd.Parameters("?Feelings").Value.ToString & ", " & sqlCmd.Parameters("?Erotic").Value.ToString & ", " & sqlCmd.Parameters("?Tension").Value.ToString & ", " & sqlCmd.Parameters("?Requirement").Value.ToString & ", " & sqlCmd.Parameters("?Actors").Value.ToString & ", " & sqlCmd.Parameters("?Dolby").Value.ToString & ", " & sqlCmd.Parameters("?HDTV").Value.ToString & ", " & sqlCmd.Parameters("?Country").Value.ToString & ", " & sqlCmd.Parameters("?Regie").Value.ToString & ", " & sqlCmd.Parameters("?Describtion").Value.ToString & ", " & sqlCmd.Parameters("?ShortDescribtion").Value.ToString
                    Select Case myex.Number
                        Case 1062
                            MyLog.Info("MyBusinessLayer: Your importer tried to add a duplicate entry: {0}", errorRow)
                            Exit Select
                        Case 1406
                            MyLog.Info("MyBusinessLayer: Your importer tried to add a too much info: {0}, {1}", errorRow, myex.Message)
                            Exit Select
                        Case Else
                            MyLog.Info("MyBusinessLayer: ExecuteInsertTvMovieProgramsMySqlCommand caused a MySqlException - {0}, {1} {2}", myex.Message, myex.Number, myex.HelpLink)
                            Exit Select
                    End Select
                Catch ex As Exception
                    MyLog.Info("MyBusinessLayer: ExecuteInsertTvMovieProgramsMySqlCommand caused an Exception - {0}, {1}", ex.Message, ex.StackTrace)
                End Try
            Next
            Return
        End Sub
        Private Shared Sub ExecuteInsertTvMovieProgramsSqlServerCommand(ByVal aProgramList As IEnumerable(Of TvMprogram), ByVal aConnection As SqlConnection, ByVal aTransaction As SqlTransaction, ByVal aDelay As Integer)
            Dim aCounter As Integer = 0
            Dim sqlCmd As New SqlCommand()
            Dim currentInserts As New List(Of TvMprogram)(aProgramList)

            sqlCmd.CommandText = "SET IDENTITY_INSERT MpTvDb.[dbo].TVMovieProgram ON; INSERT INTO MpTvDb.[dbo].TVMovieProgram (idProgram, TvMovieBewertung, Kurzkritik, Bilddateiname, Fun, Action, Feelings, Erotic, Tension, Requirement, Actors, Dolby, HDTV, Country, Regie, Describtion, ShortDescribtion) VALUES (@idProgram, @TvMovieBewertung, @Kurzkritik, @Bilddateiname, @Fun, @Action, @Feelings, @Erotic, @Tension, @Requirement, @Actors, @Dolby, @HDTV, @Country, @Regie, @Describtion, @ShortDescribtion)"

            sqlCmd.Parameters.Add("idProgram", SqlDbType.Int)
            sqlCmd.Parameters.Add("TvMovieBewertung", SqlDbType.Int)
            sqlCmd.Parameters.Add("Bilddateiname", SqlDbType.VarChar)
            sqlCmd.Parameters.Add("Kurzkritik", SqlDbType.VarChar)
            sqlCmd.Parameters.Add("Fun", SqlDbType.Int)
            sqlCmd.Parameters.Add("Action", SqlDbType.Int)
            sqlCmd.Parameters.Add("Feelings", SqlDbType.Int)
            sqlCmd.Parameters.Add("Erotic", SqlDbType.Int)
            sqlCmd.Parameters.Add("Tension", SqlDbType.Int)
            sqlCmd.Parameters.Add("Requirement", SqlDbType.Int)
            sqlCmd.Parameters.Add("Actors", SqlDbType.Text)
            sqlCmd.Parameters.Add("Dolby", SqlDbType.Bit)
            sqlCmd.Parameters.Add("HDTV", SqlDbType.Bit)
            sqlCmd.Parameters.Add("Country", SqlDbType.VarChar)
            sqlCmd.Parameters.Add("Regie", SqlDbType.VarChar)
            sqlCmd.Parameters.Add("Describtion", SqlDbType.Text)
            sqlCmd.Parameters.Add("ShortDescribtion", SqlDbType.Text)

            Try
                sqlCmd.Connection = aConnection
                ' Prepare the command since we will reuse it quite often
                ' sqlCmd.Prepare(); <-- this would need exact param field length definitions
                sqlCmd.Transaction = aTransaction
                sqlCmd.Prepare()
            Catch ex As Exception
                Log.Info("MyBusinessLayer: ExecuteInsertProgramsSqlServerCommand - Prepare caused an Exception - {0}", ex.Message)
            End Try
            MyLog.Info("MSSQL: " & sqlCmd.CommandText.ToString)


            For Each prog As TvMprogram In currentInserts
                sqlCmd.Parameters("idProgram").Value = prog.idProgram
                sqlCmd.Parameters("TvMovieBewertung").Value = prog.Bewertung
                sqlCmd.Parameters("Bilddateiname").Value = If(Not String.IsNullOrEmpty(prog.Bilddateiname), prog.Bilddateiname, String.Empty)
                sqlCmd.Parameters("Kurzkritik").Value = If(Not String.IsNullOrEmpty(prog.Kurzkritik), prog.Kurzkritik, String.Empty)
                sqlCmd.Parameters("Fun").Value = prog.Spass
                sqlCmd.Parameters("Action").Value = prog.Action
                sqlCmd.Parameters("Feelings").Value = prog.Gefuhl
                sqlCmd.Parameters("Erotic").Value = prog.Erotik
                sqlCmd.Parameters("Tension").Value = prog.Spannung
                sqlCmd.Parameters("Requirement").Value = prog.Anspruch
                sqlCmd.Parameters("Actors").Value = If(Not String.IsNullOrEmpty(prog.Darsteller), prog.Darsteller, String.Empty)
                sqlCmd.Parameters("Dolby").Value = prog.KzDolbyDigital
                sqlCmd.Parameters("HDTV").Value = prog.KzHDTV
                sqlCmd.Parameters("Country").Value = If(Not String.IsNullOrEmpty(prog.Herstellungsland), prog.Herstellungsland, String.Empty)
                sqlCmd.Parameters("Regie").Value = If(Not String.IsNullOrEmpty(prog.Regie), prog.Regie, String.Empty)
                sqlCmd.Parameters("Describtion").Value = If(Not String.IsNullOrEmpty(prog.Beschreibung), prog.Beschreibung, String.Empty)
                sqlCmd.Parameters("ShortDescribtion").Value = If(Not String.IsNullOrEmpty(prog.KurzBeschreibung), prog.KurzBeschreibung, String.Empty)

                'Log Ausgabe des Insert string, für debug Zwecke
                'Dim _LogCmd As String = sqlCmd.CommandText

                'For Each p As SqlParameter In sqlCmd.Parameters
                '    Try
                '        'MyLog.Info(p.ParameterName & " (" & p.SqlDbType.ToString & "): " & p.Value.ToString())
                '        _LogCmd = Replace(_LogCmd, "@" & p.ParameterName, p.Value.ToString)
                '    Catch ex As Exception
                '        'MyLog.Error(p.ParameterName & " (" & p.SqlDbType.ToString & "): ERROR !!!!")
                '        _LogCmd = Replace(_LogCmd, "@" & p.ParameterName, "ERROR !!!!")
                '    End Try
                'Next

                'MyLog.Info(_LogCmd)

                Try
                    ' Finally insert all our data
                    sqlCmd.ExecuteNonQuery()
                    aCounter += 1
                    ' Avoid I/O starving
                    If aCounter Mod 2 = 0 Then
                        Thread.Sleep(aDelay)
                    End If
                Catch msex As SqlException
                    Dim errorRow As String = sqlCmd.Parameters("idProgram").Value.ToString & ", " & sqlCmd.Parameters("TvMovieBewertung").Value.ToString & ", " & sqlCmd.Parameters("Kurzkritik").Value.ToString & ", " & sqlCmd.Parameters("Bilddateiname").Value.ToString & ", " & sqlCmd.Parameters("Fun").Value.ToString & ", " & sqlCmd.Parameters("Action").Value.ToString & ", " & sqlCmd.Parameters("Feelings").Value.ToString & ", " & sqlCmd.Parameters("Erotic").Value.ToString & ", " & sqlCmd.Parameters("Tension").Value.ToString & ", " & sqlCmd.Parameters("Requirement").Value.ToString & ", " & sqlCmd.Parameters("Actors").Value.ToString & ", " & sqlCmd.Parameters("Dolby").Value.ToString & ", " & sqlCmd.Parameters("HDTV").Value.ToString & ", " & sqlCmd.Parameters("Country").Value.ToString & ", " & sqlCmd.Parameters("Regie").Value.ToString & ", " & sqlCmd.Parameters("Describtion").Value.ToString & ", " & sqlCmd.Parameters("ShortDescribtion").Value.ToString
                    Select Case msex.Number
                        Case 2601
                            Log.Info("MyBusinessLayer: Your importer tried to add a duplicate entry: {0}", errorRow)
                            Exit Select
                        Case 8152
                            Log.Info("MyBusinessLayer: Your importer tried to add a too much info: {0}, {1}", errorRow, msex.Message)
                            Exit Select
                        Case Else
                            Log.Info("MyBusinessLayer: ExecuteInsertTvMovieProgramsSqlServerCommand caused a SqlException - {0}, {1} {2}", msex.Message, msex.Number, msex.HelpLink)
                            Exit Select
                    End Select
                Catch ex As Exception
                    Log.[Error]("MyBusinessLayer: ExecuteInsertTvMovieProgramsSqlServerCommand error - {0}, {1}", ex.Message, ex.StackTrace)
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