// JavaScript source code
import React, { Component } from "react";
import "../App.css";

class MainPage extends Component {
  constructor(props) {
    super(props);
    this.state = {
      movies: [],
      isLoading: true,
      movie: null
    };
  }
  async componentDidMount() {
    const res = await fetch("http://localhost:57386/api/movie/get");
    const data = await res.json();
    this.setState({ movies: data, isLoading: false });
  }
  async getMovie(param) {
    const res = await fetch("http://localhost:57386/api/movie/get/" + param);
    const data = await res.json();
    this.setState({ movie: data });
  }
  displayMovie = () => {
    let movie = this.state.movie;
    if (movie != null) {
      return (
        <div className="movie">
          <div className="movieDetail">
            <img src={"images" + movie.CoverPath} />
            <div>
              <p>{movie.ReleaseDate}</p>
              <h1>{movie.Title}</h1>
              <p>{movie.Country}</p>
              <p>{movie.Description}</p>
              <ul>
                {movie.Genres.map(genre => {
                  return <li>{genre}</li>;
                })}
              </ul>
              <p>{"Režisér: "+movie.Director}</p>
              <p>{"Obsazení: "+movie.Cast}</p>
            </div>
          </div>
          {movie.StreamSources.map((source, key) => {
            return <iframe key={key} src={source} />;
          })}
        </div>
      );
    } else {
      return <h1>Select movie from list</h1>;
    }
  };
  render() {
    const listLinks = this.state.movies.map(movie => (
      <li>
        <p
          onClick={() => {
            this.getMovie(movie.Id);
          }}
        >
          {movie.Title}
        </p>
      </li>
    ));

    const movieDetail = this.displayMovie();
    if (this.state.isLoading) {
      return <h1>Loading....</h1>;
    }
    return (
      <div className="main">
        <div className="movie-list">
          <ul>{listLinks}</ul>
        </div>
        {movieDetail}
      </div>
    );
  }
}
export default MainPage;
