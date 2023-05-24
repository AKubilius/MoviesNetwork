import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import Container from '@mui/material/Container';
import './Container.css'
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Paper from '@mui/material/Paper';
import Collection from '../MoviesCollection/Collection';
import Movie from '../MoviesCollection/Movie';
import Filter from '../Filter/Filter';
import SearchBar from '../SearchBar/SearchBar';
import { useState } from 'react';
import { Link } from '@mui/material';

interface Movie {
  id: number;
  title: string;
  overview: string;
  poster_path: string;
}

const SimpleContainer: React.FC = () => {
  const [movies, setMovies] = useState<Movie[]>([]);
  const Url = "https://image.tmdb.org/t/p/original"
  const handleResults = (results: Movie[]) => {
    setMovies(results);
  };
  return (
    <>
      <CssBaseline />
      <Container maxWidth="xl" className='container' sx={{ height: 'auto' }}>
        <Box sx={{ height: '300vh' }}>
          <Grid container spacing={2} sx={{ height: '100%' }}>
            <Grid item xs={12} >
              <Grid container justifyContent="center" spacing={1} sx={{ height: '100%' }}>
                <Grid key={1} item>
                  <Paper
                    sx={{
                      height: '100%',
                      position: 'relative',
                      width: 350,
                    }}>
                    <aside>
                      <Filter />
                    </aside>
                  </Paper>
                </Grid>
                <Grid key={2} item>
                  <Paper
                    sx={{
                      height: 1040,
                      width: 600
                    }}>
                    <main>
                      <Collection />
                    </main>
                  </Paper>
                </Grid>
                <Grid key={3} item>
                  <Paper
                    sx={{
                      height: '100%',
                      width: 350
                    }}>
                    <aside>
                      <SearchBar  onResults={handleResults}  />
                      <div>
                        {movies.map((movie: any) => (
                          <div key={movie.id}>
                            <Link href={`/movie/${movie.id}`} sx={{textDecoration: 'none'}}>
                              <Box
                                sx={{
                                  display: 'flex',
                                  '&:hover': { opacity: [0.9, 0.8, 0.7] }
                                }}
                              >
                                <Box
                                  component="img"
                                  sx={{
                                    margin: 1,
                                    borderRadius: 2,
                                    height: 1,
                                    width: '40%',

                                  }}
                                  alt="Movie"
                                  src={`${Url}${movie.poster_path}`}
                                />
                                <h4>{movie.title}</h4>
                              </Box>
                            </Link>
                          </div>
                        ))}
                      </div>
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
export default SimpleContainer;