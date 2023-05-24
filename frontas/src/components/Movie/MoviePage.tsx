import React, { useEffect, useState } from 'react'
import axios, { AxiosResponse } from "axios";
import { useParams } from "react-router-dom";
import { info } from 'console';

import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';

export const MoviePage = () => {

    const Image_Path = "https://image.tmdb.org/t/p/w500"
    const { id } = useParams();
    const API = "https://api.themoviedb.org/3/movie/"
    const[movies,setMovies] = useState<any>([]);

    const fetch = async () =>{
        const info = await axios.get(`${API}${id}`,
        {
            params:{
                api_key:'c9154564bc2ba422e5e0dede6af7f89b'
            }
        })
        setMovies(info)
        console.log(info)
    }
    useEffect(()=>{
        fetch()
    }, [])

   
  return (
    <div>      
        <Paper
      sx={{
        component:'img',
        position:'fixed',
        left:300,
        '&:hover': {
          opacity: [0.9, 0.8, 0.7],
        },
      }}
    >
<img src={Image_Path + movies.data.poster_path}/>


    </Paper>
    <Box
      sx={{
        width: 300,
        height: 300,
        left: 800,
        position:'fixed',
        backgroundColor: 'primary.dark',
        '&:hover': {
          backgroundColor: 'primary.main',
          opacity: [0.9, 0.8, 0.7],
        },
      }}
    />
      </div>
  )
}

