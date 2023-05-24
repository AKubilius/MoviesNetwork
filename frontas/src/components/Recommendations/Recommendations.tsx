import React, { useEffect, useState } from 'react'
import '../MoviesCollection/Collection'
import { Box } from '@mui/material'
import axios from 'axios'
import Movie from '../MoviesCollection/Movie'
import { useParams,  useNavigate  } from 'react-router-dom'

export default function Collection() {

  const [page, setPage] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(false);
  const API = 'https://api.themoviedb.org/3/'
  const type = 'discover/movie'
  const [movies, setMovies] = useState<any[]>([])
  const { id } = useParams()
  const token = `Bearer ${sessionStorage.getItem("token")}`
  const { id1, id2 } = useParams<{ id1: string; id2: string }>();

  const userIds = id2 ? `?userIds=${id1}&userIds=${id2}` : `?userIds=${id1}`;
  const fetchRecoms = async (page: number) => {
    setLoading(true);
    const { data: { recommendations } } = await axios.get(`https://localhost:7019/api/Recommendation/recommendations${userIds}`, {
      params: {
       
        page: page,
      },
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: token,
      },
    });

    setMovies((prevMovies) => [...prevMovies, ...recommendations]);
    setPage((prevPage) => prevPage + 1);
    setLoading(false);
  };

  const handleOnScroll = () => {
    const windowHeight = window.innerHeight;
    const documentHeight = document.documentElement.scrollHeight;
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;

    if (scrollTop + windowHeight >= documentHeight - 100 && !loading) {
      fetchRecoms(page);
    }
  };

  useEffect(() => {
    setMovies([]); // Clear the current list of movies
    setPage(1); // Reset the page number to 1
    fetchRecoms(1); // Fetch the new recommendationsch the list with the desired initial page number
  }, [id1, id2]);




  useEffect(() => {
    window.addEventListener('scroll', handleOnScroll);
    return () => {
      window.removeEventListener('scroll', handleOnScroll);
    };
  }, [page, loading]);

  return (
    <div className='collection'>
      {movies?.map((movie: any, index: React.Key | null | undefined) => (
        <Movie
          id={movie.id}
          poster_path={movie.poster_path}
          backdrop_path={movie.backdrop_path}
          title={movie.title}
          createdDate={movie.release_date}
          
          friends={null}
          rating = {null}
          key={index}
        />
      ))}
    </div>
  )
}