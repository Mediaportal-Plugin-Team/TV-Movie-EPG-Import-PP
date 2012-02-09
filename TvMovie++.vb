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

imports System
imports System.Diagnostics
imports System.IO
imports System.Runtime.InteropServices
imports System.Threading
imports System.Timers
imports Microsoft.Win32
Imports TvControl
Imports TvDatabase
Imports TvEngine.PowerScheduler.Interfaces
Imports TvLibrary.Interfaces
Imports TvLibrary.Log
Imports TvEngine
Imports SetupTv



Namespace TvEngine
	Public Class TvMovie
		Implements ITvServerPlugin
        Implements ITvServerPluginStartedAll

#Region "Members"

        Private _database As TvMovieDatabase
        Private _stateTimer As System.Timers.Timer
        Private _isImporting As Boolean = False
        Private Const _timerIntervall As Long = 1800000
        Private Const _localMachineRegSubKey As String = "HKEY_LOCAL_MACHINE\SOFTWARE\Ewe\TVGhost\Gemeinsames"

        Private Const _virtualStoreRegSubKey32b As String = "HKEY_CURRENT_USER\Software\Classes\VirtualStore\MACHINE\SOFTWARE\Ewe\TVGhost\Gemeinsames"

        Private Const _virtualStoreRegSubKey64b As String = "HKEY_CURRENT_USER\Software\Classes\VirtualStore\MACHINE\SOFTWARE\Wow6432Node\Ewe\TVGhost\Gemeinsames"

#End Region

#Region "Static properties"

        Private Shared Function GetRegistryValueFromValueName(ByVal valueName As String) As String
            Dim value As String = String.Empty

            Try

                Try
                    value = My.Computer.Registry.GetValue(_localMachineRegSubKey, valueName, String.Empty).ToString
                Catch ex As Exception
                    value = String.Empty
                End Try

                If String.IsNullOrEmpty(value) Then
                    If OS.GetOSType = OS.OSType.Is32Bit Then
                        value = My.Computer.Registry.GetValue(_virtualStoreRegSubKey32b, valueName, String.Empty).ToString
                    Else
                        value = My.Computer.Registry.GetValue(_virtualStoreRegSubKey64b, valueName, String.Empty).ToString
                    End If
                End If

            Catch ex As Exception
                MyLog.[Error]("TVMovie: Registry lookup for {1} failed: {0}", valueName, ex.Message)
            End Try

            If String.IsNullOrEmpty(value) Then
                MyLog.Info("TVMovie: Registry setting {1} has no value", valueName)
            End If

            Return value
        End Function

        Public Shared ReadOnly Property TVMovieProgramPath() As String
            Get
                Dim setting As Setting = TvMovieDatabase.TvBLayer.GetSetting("TvMovieInstallPath", String.Empty)
                Dim path As String = setting.Value

                'If Not File.Exists(path) Then
                '    Dim setting2 As Setting = TvMovieDatabase.TvBLayer.GetSetting("TvMovieDatabasepath", String.Empty)
                '    path = IO.Path.GetDirectoryName(setting2.Value) & "\"

                If Not IO.Directory.Exists(path) Then
                    path = GetRegistryValueFromValueName("ProgrammPath")
                    setting.Value = path
                    setting.Persist()
                End If

                'setting.Value = path
                'setting.Persist()
                'End If

                Return path
            End Get
        End Property

        ''' <summary>
        ''' Retrieves or sets the location of TVDaten.mdb - prefers manual configured path, does fallback to registry.
        ''' </summary>
        Public Shared Property DatabasePath() As String
            Get
                Dim path As String = TvMovieDatabase.TvBLayer.GetSetting("TvMoviedatabasepath", String.Empty).Value

                If Not File.Exists(path) Then
                    path = GetRegistryValueFromValueName("DBDatei")
                End If

                Return path
            End Get
            Set(ByVal value As String)
                Dim path As String = Value

                'If passed path is invalid
                If Not File.Exists(path) Then
                    path = DatabasePath
                End If

                Dim setting As Setting = TvMovieDatabase.TvBLayer.GetSetting("TvMoviedatabasepath")
                setting.Value = path
                setting.Persist()
            End Set
        End Property

#End Region

#Region "Powerscheduler handling"

        Private Sub SetStandbyAllowed(ByVal allowed As Boolean)
            If GlobalServiceProvider.Instance.IsRegistered(Of IEpgHandler)() Then
                GlobalServiceProvider.Instance.[Get](Of IEpgHandler)().SetStandbyAllowed(Me, allowed, 1800)
                If Not allowed Then
                    MyLog.Debug("TVMovie: Telling PowerScheduler standby is allowed: {0}, timeout is 30 minutes", allowed)
                End If
            End If
        End Sub

        Private Sub RegisterForEPGSchedule()
            ' Register with the EPGScheduleDue event so we are informed when
            ' the EPG wakeup schedule is due.
            If GlobalServiceProvider.Instance.IsRegistered(Of IEpgHandler)() Then
                Dim handler As IEpgHandler = GlobalServiceProvider.Instance.[Get](Of IEpgHandler)()
                If handler IsNot Nothing Then
                    AddHandler handler.EPGScheduleDue, New EPGScheduleHandler(AddressOf EPGScheduleDue)
                    MyLog.Debug("TVMovie: registered with PowerScheduler EPG handler")
                    Return
                End If
            End If
            MyLog.Debug("TVMovie: NOT registered with PowerScheduler EPG handler")
        End Sub

        Private Sub EPGScheduleDue()
            SpawnImportThread()
        End Sub

#End Region

#Region "Import methods"

        Private Sub ImportThread()
            Try
                _isImporting = True

                Try
                    _database = New TvMovieDatabase()
                    _database.Connect()
                Catch generatedExceptionName As Exception
                    MyLog.[Error]("TVMovie: Import enabled but the ClickFinder database was not found.")
                    Return
                End Try

                'Mylog.Debug("TVMovie: Checking database");
                Try
                    If _database.NeedsImport Then
                        SetStandbyAllowed(False)

                        Dim updateDuration As Long = _database.LaunchTVMUpdater(True)
                        If updateDuration < 1200 Then
                            ' Updating a least a few programs should take more than 20 seconds
                            If updateDuration > 20 Then
                                _database.Import()
                            Else
                                MyLog.Info("TVMovie: Import skipped because there was no new data.")
                            End If
                        Else
                            MyLog.Info("TVMovie: Import skipped because the update process timed out / has been aborted.")
                        End If
                    End If
                Catch ex As Exception
                    MyLog.Info("TvMovie plugin error:")
                    MyLog.Write(ex)
                End Try
            Finally
                _isImporting = False
                SetStandbyAllowed(True)
            End Try
        End Sub

        Private Sub StartImportThread(ByVal source As Object, ByVal e As ElapsedEventArgs)
            'TODO: check stateinfo
            SpawnImportThread()
        End Sub

        Private Sub SpawnImportThread()
            Try
                Dim layer As New TvBusinessLayer()
                If layer.GetSetting("TvMovieEnabled", "false").Value <> "true" Then
                    Return
                End If
            Catch ex1 As Exception
                MyLog.[Error]("TVMovie: Error checking enabled status - {0},{1}", ex1.Message, ex1.StackTrace)
            End Try

            If Not _isImporting Then
                Try
                    Dim importThread__1 As New Thread(New ThreadStart(AddressOf ImportThread))
                    importThread__1.Name = "TV Movie importer"
                    importThread__1.IsBackground = True
                    importThread__1.Priority = ThreadPriority.Lowest
                    importThread__1.Start()
                Catch ex2 As Exception
                    MyLog.[Error]("TVMovie: Error spawing import thread - {0},{1}", ex2.Message, ex2.StackTrace)
                End Try
            End If
        End Sub

        Private Sub StartStopTimer(ByVal startNow As Boolean)
            If startNow Then
                If _stateTimer Is Nothing Then
                    _stateTimer = New System.Timers.Timer()
                    AddHandler _stateTimer.Elapsed, New ElapsedEventHandler(AddressOf StartImportThread)
                    _stateTimer.Interval = _timerIntervall
                    _stateTimer.AutoReset = True

                    GC.KeepAlive(_stateTimer)
                End If
                _stateTimer.Start()
                _stateTimer.Enabled = True
            Else
                _stateTimer.Enabled = False
                _stateTimer.[Stop]()
                MyLog.Debug("TVMovie: background import timer stopped")
            End If
        End Sub

#End Region

#Region "ITvServerPlugin Members"

        Public ReadOnly Property Name() As String Implements ITvServerPlugin.Name
            Get
                Return "TV Movie EPG import++"
            End Get
        End Property

        Public ReadOnly Property Version() As String Implements ITvServerPlugin.Version
            Get
                Return "1.1.6.0"
            End Get
        End Property

        Public ReadOnly Property Author() As String Implements ITvServerPlugin.Author
            Get
                Return "rtv, Scrounger"
            End Get
        End Property

        Public ReadOnly Property MasterOnly() As Boolean Implements ITvServerPlugin.MasterOnly
            Get
                Return False
            End Get
        End Property
        <CLSCompliant(False)> _
        Public Sub Start(ByVal controller As IController) Implements ITvServerPlugin.Start
            StartStopTimer(True)
        End Sub

        Public Sub StartedAll() Implements ITvServerPluginStartedAll.StartedAll
            RegisterForEPGSchedule()
        End Sub

        Public Sub [Stop]() Implements ITvServerPlugin.[Stop]
            If _database IsNot Nothing Then
                _database.Canceled = True
            End If
            If _stateTimer IsNot Nothing Then
                StartStopTimer(False)
                _stateTimer.Dispose()
            End If
        End Sub
        <CLSCompliant(False)> _
        Public ReadOnly Property Setup() As SectionSettings Implements ITvServerPlugin.Setup
            Get
                Return New SetupTv.Sections.TvMovieSetup
            End Get
        End Property

#End Region

    End Class

#Region "32/64bit check"
    Public Class OS
        Public Enum OSType
            Is32Bit
            Is64Bit
        End Enum

        ''' <summary>
        ''' Gibt den Typ des Betriebssystems zurück (32 oder 64 Bit)
        ''' </summary>
        <System.ComponentModel.Description("Gibt den Typ des Betriebssystems zurück, also 32 oder 64 Bit")> _
        Public Shared Function GetOSType() As OSType
            Return IIf(IntPtr.Size = 4, OS.OSType.Is32Bit, OS.OSType.Is64Bit)
        End Function
    End Class
#End Region

End Namespace
