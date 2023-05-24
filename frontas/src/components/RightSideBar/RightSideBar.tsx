import React, { useEffect, useState } from 'react'
import Box from '@mui/material/Box';
import More from '@mui/icons-material/MoreHoriz';
import './RightSideBar.css'
import Grid from '@mui/material/Grid';
import Paper from '@mui/material/Paper';
import Avatar from '@mui/material/Avatar';
import { deepOrange, green } from '@mui/material/colors';
import Rating from '@mui/material/Rating';
import Button from '@mui/material/Button';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import Posts from '../Post/Posts';
import axios, { AxiosRequestConfig } from 'axios'
import RightMovie from './RightMovie'
import { title } from 'process';

export const RightSideBar = () => {

  const API = 'https://api.themoviedb.org/3/'
  const type = 'trending/all/day'
  const [movies, setMovies] = useState<any[]>([])


  const fetch = async () => {
    const { data:{results} } = await axios.get(`${API}${type}`,
      {
        params: {
          api_key: 'c9154564bc2ba422e5e0dede6af7f89b',
          page: '1',
          language:'lt-LT'
        }
      })
    setMovies(results)
  }
  useEffect(() => {
    fetch()
  }, [])



  return (
    <div>
        <p className='todays-actual-header'>Šios dienos aktualiausi</p>
        <Grid container  columnSpacing={{ xs: 5, sm: 5, md: 1 }}>
          {movies?.slice(0,6).map((movie: any, index: React.Key | null | undefined) => (
            <Grid key={index} item xs={6}>
                <RightMovie
                    id={movie.id}
                    key={movie.id}
                    title={movie.title}
                    posterPath={movie.poster_path}
                    createdDate ={movie.release_date}
                />
                </Grid>
            ))}
        </Grid>
    </div>
  )
}
