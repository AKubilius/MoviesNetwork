import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import Container from '@mui/material/Container';
import './Container.css'
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Paper from '@mui/material/Paper';
import Avatar from '@mui/material/Avatar';
import { deepOrange, green } from '@mui/material/colors';
import Rating from '@mui/material/Rating';
import User from '../Users/User';
import Button from '@mui/material/Button';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import Posts from '../Post/Posts';
import { RightSideBar } from '../RightSideBar/RightSideBar';
import Users from '../Users/Users';
import UserSide from '../Users/UserSide';
import Collection from '../MoviesCollection/Collection';
import Movie from '../MoviesCollection/Movie';
import Collenction from '../MoviesCollection/Collection';
import Filter from '../Filter/Filter';
import { useParams } from 'react-router-dom';
import SearchBar from '../SearchBar/SearchBar';
import { useState } from 'react';
import AddFriends from '../Recommendations/AddFriends';
import Recommendations from '../Recommendations/Recommendations';

interface Movie {
    id: number;
    title: string;
    overview: string;
    poster_path: string;
  }

const RecomContainer: React.FC = () => {
    const [movies, setMovies] = useState<Movie[]>([]);
    const Url = "https://image.tmdb.org/t/p/original"
    const handleResults = (results: Movie[]) => {
        setMovies(results);
      };
    return (

       <>
            <CssBaseline />
            <Container maxWidth="xl" className='container' sx={{height:'auto'}}>
                <Box sx={{  height: '300vh'}}>
                    <Grid container spacing={2}  sx={{height: '100%'}}>
                        <Grid item xs={12} >
                            <Grid container justifyContent="center" spacing={1} sx={{height: '100%'}}>
                                <Grid key={1} item>
                                    
                                    <Paper 
                                        sx={{
                                            height:'100%',
                                            position:'relative',                                       
                                            width: 350,
                                            backgroundColor: (theme) =>
                                                theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
                                        }}>
                                        <aside>
                                         <AddFriends/>
                                        </aside>
                                    </Paper>
                                </Grid>
                                <Grid key={2} item>
                                    <Paper
                                        sx={{
                                            height: 1040,
                                            width: 600,
                                            backgroundColor: (theme) =>
                                                theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
                                        }}>
                                         <main>
                                         <Recommendations/>
                                        </main>
                                    </Paper>
                                </Grid>
                                <Grid key={3} item>
                                    <Paper
                                        sx={{
                                            height:'100%',
                                            width: 350,
                                            backgroundColor: (theme) =>
                                                theme.palette.mode === 'dark' ? '#1A2027' : '#fff'}}>
                                        <aside>
                                       
      
     
                                        </aside>
                                    </Paper>
                                </Grid>
                            </Grid>
                        </Grid>
                    </Grid>
                </Box>
            </Container>
            </>
    );
}
export default RecomContainer;