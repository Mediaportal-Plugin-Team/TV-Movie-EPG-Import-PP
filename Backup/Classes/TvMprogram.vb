Imports System.Data.OleDb
Imports System.Text
Imports System.Reflection
Imports System.Data
Imports System.Collections.Generic

Namespace TvEngine

    Public Class TvMprogram

#Region "Members"

        Private _idProgram As Integer = 0

        Private _Beginn As Date = Nothing
        Private _Ende As Date = Nothing

        Private _Titel As String = String.Empty
        Private _orgTitel As String = String.Empty
        Private _SenderKennung As String = String.Empty
        Private _Darsteller As String = String.Empty
        Private _Regie As String = String.Empty
        Private _Herstellungsland As String = String.Empty
        Private _Herstellungsjahr As String = String.Empty
        Private _Kurzkritik As String = String.Empty
        Private _Bilddateiname As String = String.Empty
        Private _Beschreibung As String = String.Empty
        Private _KurzBeschreibung As String = String.Empty



        Private _KzLive As Boolean = False
        Private _KzWiederholung As Boolean = False
        Private _KzDolbyDigital As Boolean = False
        Private _KzHDTV As Boolean = False

        Private _Bewertung As Integer = 0
        Private _Spass As Integer = 0
        Private _Action As Integer = 0
        Private _Erotik As Integer = 0
        Private _Spannung As Integer = 0
        Private _Anspruch As Integer = 0
        Private _Gefuhl As Integer = 0

#End Region

#Region "Constructors"

#End Region

#Region "Public Properties"

        Public Property idProgram() As Integer
            Get
                Return _idProgram
            End Get
            Set(ByVal value As Integer)
                _idProgram = value
            End Set
        End Property
        Public Property Titel() As String
            Get
                Return _Titel
            End Get
            Set(ByVal value As String)
                _Titel = value
            End Set
        End Property
        Public Property orgTitel() As String
            Get
                Return _orgTitel
            End Get
            Set(ByVal value As String)
                _orgTitel = value
            End Set
        End Property
        Public Property Beginn() As Date
            Get
                Return _Beginn
            End Get
            Set(ByVal value As Date)
                _Beginn = value
            End Set
        End Property
        Public Property Ende() As Date
            Get
                Return _Ende
            End Get
            Set(ByVal value As Date)
                _Ende = value
            End Set
        End Property
        Public Property SenderKennung() As String
            Get
                Return _SenderKennung
            End Get
            Set(ByVal value As String)
                _SenderKennung = value
            End Set
        End Property
        Public Property Bewertung() As Integer
            Get
                Return _Bewertung
            End Get
            Set(ByVal value As Integer)
                _Bewertung = value
            End Set
        End Property
        Public Property KzLive() As Boolean
            Get
                Return _KzLive
            End Get
            Set(ByVal value As Boolean)
                _KzLive = value
            End Set
        End Property
        Public Property KzWiederholung() As Boolean
            Get
                Return _KzWiederholung
            End Get
            Set(ByVal value As Boolean)
                _KzWiederholung = value
            End Set
        End Property
        Public Property KzDolbyDigital() As Boolean
            Get
                Return _KzDolbyDigital
            End Get
            Set(ByVal value As Boolean)
                _KzDolbyDigital = value
            End Set
        End Property
        Public Property KzHDTV() As Boolean
            Get
                Return _KzHDTV
            End Get
            Set(ByVal value As Boolean)
                _KzHDTV = value
            End Set
        End Property
        Public Property Darsteller() As String
            Get
                Return _Darsteller
            End Get
            Set(ByVal value As String)
                _Darsteller = value
            End Set
        End Property
        Public Property Regie() As String
            Get
                Return _Regie
            End Get
            Set(ByVal value As String)
                _Regie = value
            End Set
        End Property
        Public Property Herstellungsland() As String
            Get
                Return _Herstellungsland
            End Get
            Set(ByVal value As String)
                _Herstellungsland = value
            End Set
        End Property
        Public Property Kurzkritik() As String
            Get
                Return _Kurzkritik
            End Get
            Set(ByVal value As String)
                _Kurzkritik = value
            End Set
        End Property
        Public Property Bilddateiname() As String
            Get
                Return _Bilddateiname
            End Get
            Set(ByVal value As String)
                _Bilddateiname = value
            End Set
        End Property


        Public Property Spass() As Integer
            Get
                Return _Spass
            End Get
            Set(ByVal value As Integer)
                _Spass = value
            End Set
        End Property
        Public Property Action() As Integer
            Get
                Return _Action
            End Get
            Set(ByVal value As Integer)
                _Action = value
            End Set
        End Property
        Public Property Gefuhl() As Integer
            Get
                Return _Gefuhl
            End Get
            Set(ByVal value As Integer)
                _Gefuhl = value
            End Set
        End Property
        Public Property Erotik() As Integer
            Get
                Return _Erotik
            End Get
            Set(ByVal value As Integer)
                _Erotik = value
            End Set
        End Property
        Public Property Spannung() As Integer
            Get
                Return _Spannung
            End Get
            Set(ByVal value As Integer)
                _Spannung = value
            End Set
        End Property
        Public Property Anspruch() As Integer
            Get
                Return _Anspruch
            End Get
            Set(ByVal value As Integer)
                _Anspruch = value
            End Set
        End Property
        Public Property Beschreibung() As String
            Get
                Return _Beschreibung
            End Get
            Set(ByVal value As String)
                _Beschreibung = value
            End Set
        End Property

        Public Property KurzBeschreibung() As String
            Get
                Return _KurzBeschreibung
            End Get
            Set(ByVal value As String)
                _KurzBeschreibung = value
            End Set
        End Property

#End Region

#Region "Retrieval"
        ''' <summary>
        ''' Retrieve List(of TvMprogram), SqlString vorbereitet (FROM Sendungen INNER JOIN SendungenDetails)
        ''' SQLStringAppendix = WHERE, ORDER BY, etc.
        ''' DatabasePath = Pfad zur Datenbank (absolut)
        ''' </summary>
        Public Overloads Shared Function RetrieveList(ByVal SQLStringAppendix As String, ByVal ChannelName As String) As List(Of TvMprogram)

            Dim _dataProviderString As String = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Mode=Share Deny None;Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;", TvMovie.DatabasePath)
            Dim _databaseConnection As OleDbConnection = New OleDbConnection(_dataProviderString)
            Dim _databaseTransaction As OleDbTransaction = Nothing

            Dim _TvMProgramList As New List(Of TvMprogram)

            Dim _SqlBuilder As New StringBuilder()
            Dim _SqlString As String = String.Empty

            _SqlBuilder.Append("SELECT Sendungen.Titel, Sendungen.Beginn, Sendungen.Ende, Sendungen.SenderKennung,")
            _SqlBuilder.Append(" Sendungen.Bewertung, Sendungen.KzLive, Sendungen.KzWiederholung, Sendungen.KzDolby,")
            _SqlBuilder.Append(" Sendungen.KzDolbyDigital, Sendungen.KzDolbySurround, Sendungen.KzHDTV, Sendungen.Bilddateiname,")
            _SqlBuilder.Append(" SendungenDetails.Darsteller, Sendungen.Regie, Sendungen.Herstellungsland, Sendungen.Herstellungsjahr,")
            _SqlBuilder.Append(" Sendungen.Kurzkritik, Sendungen.Bewertungen, SendungenDetails.Beschreibung, Sendungen.KurzBeschreibung, Sendungen.Originaltitel")
            _SqlBuilder.Append(" FROM Sendungen LEFT JOIN SendungenDetails ON Sendungen.Pos = SendungenDetails.Pos {0};")
            _SqlString = String.Format(_SqlBuilder.ToString(), SQLStringAppendix)

            Using databaseCommand As New OleDbCommand(_SqlString, _databaseConnection)
                Try
                    _databaseConnection.Open()
                    ' The main app might change epg details while importing
                    _databaseTransaction = _databaseConnection.BeginTransaction(IsolationLevel.ReadCommitted)
                    databaseCommand.Transaction = _databaseTransaction

                    Using reader As OleDbDataReader = databaseCommand.ExecuteReader(CommandBehavior.SequentialAccess)

                        While reader.Read()
                            Dim _TvMprogram As New TvMprogram

                            _TvMprogram.Titel = reader(0).ToString
                            _TvMprogram.Beginn = CDate(reader(1).ToString)
                            _TvMprogram.Ende = CDate(reader(2).ToString)
                            _TvMprogram.SenderKennung = reader(3).ToString

                            _TvMprogram.Bewertung = CInt(reader(4).ToString)
                            _TvMprogram.KzLive = CBool(reader(5).ToString)
                            _TvMprogram.KzWiederholung = CBool(reader(6).ToString)

                            'Wenn HD Sender -> Dolby = true, Hd = true
                            If InStr(ChannelName, " HD") > 0 Then
                                _TvMprogram.KzDolbyDigital = True
                                _TvMprogram.KzHDTV = True
                            Else
                                'DolbyDigital übergeben
                                If CBool(reader(7).ToString) = True Or CBool(reader(8).ToString) = True Or CBool(reader(9).ToString) = True Then
                                    _TvMprogram.KzDolbyDigital = True
                                Else
                                    _TvMprogram.KzDolbyDigital = False
                                End If

                                _TvMprogram.KzHDTV = CBool(reader(10).ToString)
                            End If

                            _TvMprogram.Bilddateiname = reader(11).ToString
                            _TvMprogram.Darsteller = Replace(reader(12).ToString, ";", ", ")
                            _TvMprogram.Regie = Replace(reader(13).ToString, ";", ", ")
                            _TvMprogram.Herstellungsland = Replace(reader(14).ToString, ";", " ")
                            '_TvMprogram.Herstellungsjahr = CDate(reader(15).ToString)
                            'aus(program)

                            _TvMprogram.Kurzkritik = reader(16).ToString


                            'Bewertungen string zerlegen
                            If Not String.IsNullOrEmpty(reader(17).ToString) Then
                                Dim s As String = reader(17).ToString
                                Dim words As String() = s.Split(New Char() {";"c})

                                For Each word As String In words
                                    'MsgBox(Left(word, InStr(word, "=") - 1))
                                    'MsgBox(CInt(Right(word, word.Length - InStr(word, "="))))

                                    Select Case Left(word, InStr(word, "=") - 1)
                                        Case Is = "Spaß"
                                            _TvMprogram.Spass = CInt(Right(word, word.Length - InStr(word, "=")))
                                        Case Is = "Action"
                                            _TvMprogram.Action = CInt(Right(word, word.Length - InStr(word, "=")))
                                        Case Is = "Erotik"
                                            _TvMprogram.Erotik = CInt(Right(word, word.Length - InStr(word, "=")))
                                        Case Is = "Spannung"
                                            _TvMprogram.Spannung = CInt(Right(word, word.Length - InStr(word, "=")))
                                        Case Is = "Anspruch"
                                            _TvMprogram.Anspruch = CInt(Right(word, word.Length - InStr(word, "=")))
                                        Case Is = "Gefühl"
                                            _TvMprogram.Gefuhl = CInt(Right(word, word.Length - InStr(word, "=")))
                                    End Select
                                Next
                            End If

                            _TvMprogram.Beschreibung = Replace(reader(18).ToString, "<br>", vbNewLine)
                            _TvMprogram.KurzBeschreibung = reader(19).ToString
                            _TvMprogram.orgTitel = reader(20).ToString

                            _TvMProgramList.Add(_TvMprogram)
                        End While

                        _databaseTransaction.Commit()
                        reader.Close()
                    End Using

                Catch ex As OleDbException
                    _databaseTransaction.Rollback()
                    MyLog.Info("TvMprogram: [RetrieveList]: Error accessing TV Movie Clickfinder database - import canceled")
                    MyLog.[Error]("TvMprogram: [RetrieveList:] exception err:{0} stack:{1}", ex.Message, ex.StackTrace)

                Catch ex1 As Exception
                    Try
                        _databaseTransaction.Rollback()
                    Catch generatedExceptionName As Exception
                    End Try
                    MyLog.[Error]("TvMprogram: [RetrieveList:] exception err:{0} stack:{1}", ex1.Message, ex1.StackTrace)
                Finally
                    _databaseConnection.Close()
                End Try
            End Using

            _databaseConnection.Close()

            Return _TvMProgramList

        End Function
#End Region

    End Class

    Public Class TvMprogram_GroupByIdprogram
        Implements IEqualityComparer(Of TvMprogram)

        Private _PropertyInfo As PropertyInfo
        Public Function Equals1(ByVal x As TvMprogram, ByVal y As TvMprogram) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of TvMprogram).Equals
            If x.idProgram = y.idProgram Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetHashCode1(ByVal obj As TvMprogram) As Integer Implements System.Collections.Generic.IEqualityComparer(Of TvMprogram).GetHashCode
            Return obj.idProgram.GetHashCode()
        End Function
    End Class
End Namespace