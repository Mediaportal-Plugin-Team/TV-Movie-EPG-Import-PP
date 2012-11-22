Imports Gentle.Framework

Public Class Helper
    Public Shared Sub CreateClearTvMovieProgramTable()
        'Tabellen erstellen / clear
        If Gentle.Framework.Broker.ProviderName = "MySQL" Then
            'Provider: MySQL
            Try
                Broker.Execute("DROP TABLE mptvdb.tvmovieprogram")
                Broker.Execute("CREATE TABLE mptvdb.TVMovieProgram ( idTVMovieProgram INT NOT NULL AUTO_INCREMENT , idProgram INT NOT NULL DEFAULT 0 , TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT(1) NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , needsUpdate BIT(1) NOT NULL DEFAULT 1 , Dolby BIT(1) NOT NULL DEFAULT 0 , HDTV BIT(1) NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Year DATETIME NOT NULL , Describtion TEXT , ShortDescribtion TEXT , FileName VARCHAR(255) , PRIMARY KEY (idTVMovieProgram) )")
                MyLog.[Debug]("TVMovie++: TvMovieProgram table cleared")
            Catch ex As Exception
                'Falls die Tabelle nicht existiert, abfangen & erstellen
                Broker.Execute("CREATE TABLE mptvdb.TVMovieProgram ( idTVMovieProgram INT NOT NULL AUTO_INCREMENT , idProgram INT NOT NULL DEFAULT 0 , TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT(1) NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , needsUpdate BIT(1) NOT NULL DEFAULT 1 , Dolby BIT(1) NOT NULL DEFAULT 0 , HDTV BIT(1) NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Year DATETIME NOT NULL , Describtion TEXT , ShortDescribtion TEXT , FileName VARCHAR(255) , PRIMARY KEY (idTVMovieProgram) )")
                MyLog.[Debug]("TVMovie++: TvMovieProgram table created")
            End Try

            Try
                'Table TvMovieSeriesMapping anlegen
                Broker.Execute("CREATE  TABLE mptvdb.TvMovieSeriesMapping ( idSeries INT NOT NULL , EpgTitle VARCHAR(255) , PRIMARY KEY (idSeries) )")
                MyLog.[Debug]("TVMovie++: TvMovieSeriesMapping table created")
            Catch ex As Exception
                'existiert bereits
                MyLog.[Debug]("TVMovie++: TvMovieSeriesMapping table exist")
            End Try
        Else
            'Provider: MSSQL
            Try
                Broker.Execute("DROP TABLE mptvdb.[dbo].tvmovieprogram")
                Broker.Execute("CREATE TABLE MpTvDb.[dbo].TVMovieProgram ( idTVMovieProgram int NOT NULL IDENTITY, idProgram INT NOT NULL DEFAULT 0 , TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , needsUpdate BIT NOT NULL DEFAULT 1 , Dolby BIT NOT NULL DEFAULT 0 , HDTV BIT NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Year DATETIME NOT NULL , Describtion TEXT , ShortDescribtion TEXT , FileName VARCHAR(255) , PRIMARY KEY (idTVMovieProgram))")
            Catch ex As Exception
                'Falls die Tabelle nicht existiert, abfangen & erstellen
                Broker.Execute("CREATE TABLE MpTvDb.[dbo].TVMovieProgram ( idTVMovieProgram int NOT NULL IDENTITY, idProgram INT NOT NULL DEFAULT 0 , TVMovieBewertung INT NOT NULL DEFAULT 0 , FanArt VARCHAR(255) , idSeries INT NOT NULL DEFAULT 0 , SeriesPosterImage VARCHAR(255) , idEpisode VARCHAR(15) , EpisodeImage VARCHAR(255) , local BIT NOT NULL DEFAULT 0 , idMovingPictures INT NOT NULL DEFAULT 0 , idVideo INT NOT NULL DEFAULT 0 , KurzKritik VARCHAR(255) , BildDateiname VARCHAR(32) , Cover VARCHAR(512) , Fun INT NOT NULL DEFAULT 0 , Action INT NOT NULL DEFAULT 0 , Feelings INT NOT NULL DEFAULT 0 , Erotic INT NOT NULL DEFAULT 0 , Tension INT NOT NULL DEFAULT 0 , Requirement INT NOT NULL DEFAULT 0 , Actors TEXT , needsUpdate BIT NOT NULL DEFAULT 1 , Dolby BIT NOT NULL DEFAULT 0 , HDTV BIT NOT NULL DEFAULT 0 , Country VARCHAR(50) , Regie VARCHAR(50) , Year DATETIME NOT NULL , Describtion TEXT , ShortDescribtion TEXT , FileName VARCHAR(255) , PRIMARY KEY (idTVMovieProgram))")
            End Try

            Try
                'Table TvMovieSeriesMapping anlegen
                Broker.Execute("CREATE  TABLE mptvdb.[dbo].TvMovieSeriesMapping ( idSeries INT NOT NULL , EpgTitle VARCHAR(255) , PRIMARY KEY (idSeries) )")
                MyLog.[Debug]("TVMovie++: TvMovieSeriesMapping table created")
            Catch ex As Exception
                'existiert bereits
                MyLog.[Debug]("TVMovie++: TvMovieSeriesMapping table exist")
            End Try
        End If
    End Sub

End Class
