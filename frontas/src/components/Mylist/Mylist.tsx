import {useEffect, useState} from 'react'
import axios from 'axios'
import React from 'react';
import { Link, useParams } from "react-router-dom";
import Box from '@mui/material/Box/Box';
import ListMovie from './ListMovie';

export default function BasicTable() {
 
  const {userName} = useParams();
  console.log(userName)
    const API = 'https://api.themoviedb.org/3/'
    const[movies,setMovies] = useState<any[]>([])
    const token = `Bearer ${sessionStorage.getItem("token")}`
    const fetch = async () =>{
      const {data} = await axios.get(`https://localhost:7019/List/Mylist${userName ? `/${userName}` : ""}`,
      {
          headers: {
              'Content-Type': 'application/json',
              Accept: 'application/json',
              Authorization: token
            },
            
      })
      data.map((data: { movieID: any; }) => fetchMovies(data?.movieID))
    }
    const fetchMovies = async  (id: any) => {
      try {
        const {data} = await axios.get(`${API}/movie/${id}`, {
          params:{
              api_key: 'c9154564bc2ba422e5e0dede6af7f89b',
          }
        })
        console.log(data)
    
        setMovies((prevState) => [...prevState, data])
      } catch(e) {}
      
    }
    useEffect(() => {
      fetch()
    }, [])
  return (
    <div className='collection'>
      {movies?.map((movie: any, index: React.Key | null | undefined) => (
                <ListMovie
                    id={movie.id}
                    poster_path={movie.poster_path}
                    backdrop_path={movie.backdrop_path}
                    title={movie.title}
                    createdDate ={movie.release_date}
                    key={index}
                />
            ))}
    </div>
  );
}