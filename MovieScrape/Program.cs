using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HtmlAgilityPack;
using System.Net;
using MovieScrape.Model;

namespace MovieScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://kinodum.cz/filmy-online/page/";
            int pageCount = 48;


            //List<string> links = new List<string>();
            //links.Add("http://kinodum.cz/filmy-online/sci-fi/650-blade-runner-2049.html");
            //links.Add("http://kinodum.cz/filmy-online/drama/669-souboj-pohlavi.html");
            //links.Add("http://kinodum.cz/filmy-online/komedie/663-srden-vas-vitame.html");

            //get links from website
            List< string > links = ScrapeLinks(url, pageCount);

            //scrape movies
            ScrapeMovies(links);


            Console.ReadLine();
        }

        private static void ScrapeMovies(List<string> links)
        {
            var web = new HtmlWeb();
            var document = new HtmlDocument();
            Genre Genre = new Genre();
            Movie Movie = new Movie();
            StreamSource StreamSourceLinks = new StreamSource();
            int index = 0;
            foreach (var link in links)
            {
                try
                {
                    document = web.Load(link);
                }
                catch (Exception e)
                {
                    LogError(e, link);
                    continue;
                }
                Guid gId = Guid.NewGuid();

                try
                {
                    Movie.Id = gId;
                    //get title of movie
                    string title = document.DocumentNode.SelectSingleNode("//h1[@class='title']/span[@itemprop='name']").InnerHtml;
                    Movie.Title = title;
                    //get description
                    string description = document.DocumentNode.SelectSingleNode("//div[@itemprop='description']").InnerHtml;
                    Movie.Description = description;
                    //get year of release
                    string yearOfRelease = document.DocumentNode.SelectSingleNode("//span[@itemprop='copyrightYear']/a").InnerHtml;
                    Movie.ReleaseDate = yearOfRelease;

                    //get other details of the movie
                    var details = document.DocumentNode.SelectNodes("//ul[@class='detail']/div/li");

                    //get all genres
                    var genres = details[2].Descendants("a");
                    foreach (var item in genres)
                    {
                        Genre.MovieId = gId;
                        Genre.GenreType = item.InnerHtml;
                        using (MoviesDBEntities db = new MoviesDBEntities())
                        {
                            db.Genres.Add(Genre);
                            db.SaveChanges();
                        }
                    }

                    Movie.Country = (details[1].LastChild.InnerHtml);//country               
                    Movie.Lenght = (details[3].LastChild.InnerHtml);//lenght
                    Movie.Dabing = (details[4].LastChild.InnerHtml);//dabing

                    if (details[5].FirstChild.InnerHtml == "Originální název: ")
                    {
                        Movie.OriginalTitle= (details[5].LastChild.InnerHtml);//original name
                        Movie.Director=(details[6].LastChild.InnerHtml);//director
                        Movie.Cast=(details[7].LastChild.InnerHtml);//cast
                    }
                    else
                    {
                        Movie.Director=(details[5].LastChild.InnerHtml);//director
                        Movie.Cast=(details[6].LastChild.InnerHtml);//cast
                    }

                    //get stream links
                    var streamLinks = document.DocumentNode.SelectNodes("//div[@id='k_online']/iframe");
                    foreach (var item in streamLinks)
                    {
                        StreamSourceLinks.MovieId = gId;
                        StreamSourceLinks.Source = item.Attributes["src"].Value;
                        using (MoviesDBEntities db = new MoviesDBEntities())
                        {
                            db.StreamSources.Add(StreamSourceLinks);
                            db.SaveChanges();
                        }
                    }

                    string imagePath = document.DocumentNode.SelectSingleNode("//div[@class='full-poster']/img[@itemprop='image']").Attributes["src"].Value;
                    imagePath = "http://kinodum.cz" + imagePath;

                    Movie.CoverPath = (SaveCoverImage(gId, imagePath));
                    using (MoviesDBEntities db = new MoviesDBEntities())
                    {
                        db.Movies.Add(Movie);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    LogError(e, link);
                    continue;
                }
                Console.WriteLine("Index: {0} Movie: {1} saved to db", index++, gId.ToString());             
            }
        }

        private static List<string> ScrapeLinks(string url, int pageCount)
        {
            var web = new HtmlWeb();
            var document = new HtmlDocument();
            //go to every page
            List<string> links = new List<string>();
            //always start on page with index 1
            for (int i = 1; i <= pageCount; i++) 
            {
                //load page
                document = web.Load(url + i.ToString());
                //get nodes which contains a tag
                var movieList = document.DocumentNode.SelectNodes("//div[@class='poster']/a[@itemprop='url']");

                foreach (var item in movieList)
                {
                    //get href value and add to list                   
                   links.Add(item.Attributes["href"].Value);
                }
            }
            return links;
        }
        public static string SaveCoverImage(Guid id, string imageURL)
        {
            //check if folder MovieCovers exists in MyDocuments if not then create
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MovieCovers")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MovieCovers"));
            }
            //create new folder named as movie guid
            if (!Directory.Exists(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MovieCovers"), id.ToString())))
            {
                Directory.CreateDirectory(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MovieCovers"), id.ToString()));
            }

            //create path with final img name 
            string downloadFolder = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MovieCovers"), id.ToString())+ @"\" + id.ToString() + @".jpg";

            //download img from url to path we created above
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(imageURL, downloadFolder);
                }
            }
            catch (Exception e)
            {
                LogError(e, imageURL, id.ToString());
            }

            Console.WriteLine("Cover downloaded for Movie: "+id.ToString());
            
            //return path but without "C:\Users\username
            return @"\MovieCovers\" + id.ToString() + @"\" + id.ToString() + @".jpg";
        } 
        public static void LogError(Exception e, string text = "", string property= "")
        {
            string message = DateTime.Now.ToString() + "\r\n" + e.ToString() + "\r\n" + text + "\r\n" + property;
            File.WriteAllText("log.txt", message);
        }
    }
}
