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

export default function SimpleContainer() {
    const [value, setValue] = React.useState<boolean>(true);
    return (

        <>
      <CssBaseline />
      <Container maxWidth="xl" className="container">
        <Box>
          <Grid container spacing={5}>
            <Grid item xs={12}>
              <Grid container justifyContent="center" spacing={5}>
                <Grid key={1} item>
                  <Paper sx={{ height: '100%', width: 350 }}>
                    <aside>
                      <UserSide />
                    </aside>
                  </Paper>
                </Grid>
                <Grid key={2} item>
                  <Paper sx={{ height: 'auto', width: 600 }}>
                    <main>
                      <Posts
                      param=""
                      />
                    </main>
                  </Paper>
                </Grid>
                <Grid key={3} item>
                  <Paper sx={{ height: '100%', width: 350 }}>
                    <aside>
                      <RightSideBar />
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