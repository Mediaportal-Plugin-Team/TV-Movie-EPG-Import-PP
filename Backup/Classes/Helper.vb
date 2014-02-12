Imports Gentle.Framework

Public Class Helper
    Public Shared Sub CreateOrClearTvMovieProgramTables()

        'Tabellen erstellen / clear
        If Gentle.Framework.Broker.ProviderName = "MySQL" Then
            'Provider: MySQL
            Try
                Broker.Execute("DROP TABLE mptvdb.tvmovieprogram")
                Broker.Execute("CREATE TABLE mptvdb.TVMovieProgram ( idProgram INT NOT NULL , TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT(1) NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , Dolby BIT(1) NOT NULL DEFAULT 0 , HDTV BIT(1) NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Describtion TEXT , ShortDescribtion TEXT , FileName VARCHAR(255) , PRIMARY KEY (idProgram) )")
            Catch ex As Exception
                'Falls die Tabelle nicht existiert, abfangen & erstellen
                Broker.Execute("CREATE TABLE mptvdb.TVMovieProgram ( idProgram INT NOT NULL , TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT(1) NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , Dolby BIT(1) NOT NULL DEFAULT 0 , HDTV BIT(1) NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Describtion TEXT , ShortDescribtion TEXT , FileName VARCHAR(255) , PRIMARY KEY (idProgram) )")
            End Try

            Try
                'Table TvMovieSeriesMapping anlegen
                Broker.Execute("CREATE  TABLE mptvdb.TvMovieSeriesMapping ( idSeries int(11) NOT NULL, disabled bit(1) NOT NULL DEFAULT b'0', TvSeriesTitle varchar(255) DEFAULT NULL, EpgTitle varchar(255) DEFAULT NULL, minSeasonNum int(11) NOT NULL DEFAULT '0', PRIMARY KEY (idSeries) )")
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieSeriesMapping table created")
            Catch ex As Exception
                'existiert bereits
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieSeriesMapping table exist")
            End Try

            Try
                'Table TVMovieEpisodeMapping anlegen
                Broker.Execute("CREATE  TABLE mptvdb.TVMovieEpisodeMapping ( idEpisode varchar(15) NOT NULL, idSeries int(11) NOT NULL, EPGEpisodeName text, seriesNum int(11) NOT NULL, episodeNum int(11) NOT NULL, PRIMARY KEY (idEpisode) )")
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TVMovieEpisodeMapping table created")
            Catch ex As Exception
                'existiert bereits
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TVMovieEpisodeMapping table exist")
            End Try
        Else
            'Provider: MSSQL
            Try
                Broker.Execute("DROP TABLE mptvdb.[dbo].TVMovieProgram")
                Broker.Execute("CREATE TABLE MpTvDb.[dbo].TVMovieProgram ( idProgram int NOT NULL IDENTITY, TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , Dolby BIT NOT NULL DEFAULT 0 , HDTV BIT NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Describtion TEXT , ShortDescribtion TEXT , FileName VARCHAR(255) , PRIMARY KEY (idProgram))")
            Catch ex As Exception
                'Falls die Tabelle nicht existiert, abfangen & erstellen
                Broker.Execute("CREATE TABLE MpTvDb.[dbo].TVMovieProgram ( idProgram int NOT NULL IDENTITY, TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , Dolby BIT NOT NULL DEFAULT 0 , HDTV BIT NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Describtion TEXT , ShortDescribtion TEXT , FileName VARCHAR(255) , PRIMARY KEY (idProgram))")
            End Try

            Try
                'Table TvMovieSeriesMapping anlegen
                Broker.Execute("CREATE  TABLE mptvdb.[dbo].TvMovieSeriesMapping ( idSeries int(11) NOT NULL, disabled bit(1) NOT NULL DEFAULT b'0', TvSeriesTitle varchar(255) DEFAULT NULL, EpgTitle varchar(255) DEFAULT NULL, minSeasonNum int(11) NOT NULL DEFAULT '0', PRIMARY KEY (idSeries) )")
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieSeriesMapping table created")
            Catch ex As Exception
                'existiert bereits
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieSeriesMapping table exist")
            End Try

            Try
                'Table TVMovieEpisodeMapping anlegen
                Broker.Execute("CREATE  TABLE mptvdb.[dbo].TvMovieEpisodeMapping ( idEpisode varchar(15) NOT NULL, idSeries int(11) NOT NULL, EPGEpisodeName text, seriesNum int(11) NOT NULL, episodeNum int(11) NOT NULL, PRIMARY KEY (idEpisode) )")
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieEpisodeMapping table created")
            Catch ex As Exception
                'existiert bereits
                MyLog.[Debug]("TVMovie: [TvMovie++ Settings]: TvMovieEpisodeMapping table exist")
            End Try
        End If
    End Sub

    Public Shared Sub DropSeriesMappingTable()

        'Tabellen erstellen / clear
        If Gentle.Framework.Broker.ProviderName = "MySQL" Then
            'Provider: MySQL
            Try
                Broker.Execute("DROP TABLE mptvdb.TvMovieSeriesMapping")
            Catch ex As Exception
                MyLog.Error("TVMovie: [TvMovie++ Settings]: MySQL dropping TvMovieSeriesMapping table error: {0}, stack: {1}", ex.Message, ex.StackTrace)
            End Try
        Else
            Try
                Broker.Execute("DROP TABLE mptvdb.[dbo].TvMovieSeriesMapping")
            Catch ex As Exception
                MyLog.Error("TVMovie: [TvMovie++ Settings]: MSSQL dropping TvMovieSeriesMapping table error: {0}, stack: {1}", ex.Message, ex.StackTrace)
            End Try
        End If
    End Sub

    Public Shared Sub DropEpisodeMappingTable()

        'Tabellen erstellen / clear
        If Gentle.Framework.Broker.ProviderName = "MySQL" Then
            'Provider: MySQL
            Try
                Broker.Execute("DROP TABLE mptvdb.TvMovieEpisodeMapping")
            Catch ex As Exception
                MyLog.Error("TVMovie: [TvMovie++ Settings]: MySQL dropping TvMovieEpisodeMapping table error: {0}, stack: {1}", ex.Message, ex.StackTrace)
            End Try
        Else
            Try
                Broker.Execute("DROP TABLE mptvdb.[dbo].TvMovieEpisodeMapping")
            Catch ex As Exception
                MyLog.Error("TVMovie: [TvMovie++ Settings]: MSSQL dropping TvMovieEpisodeMapping table error: {0}, stack: {1}", ex.Message, ex.StackTrace)
            End Try
        End If
    End Sub

End Class
