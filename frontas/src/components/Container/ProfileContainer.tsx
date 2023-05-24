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
import { Profile } from '../Profile/Profile';
import { useParams, useLocation } from 'react-router-dom';
import MyList from '../Mylist/Mylist';
import { useEffect } from 'react';
import Callendar from '../Callendar/Callendar';
import ChallengesBox from '../Challenge/ChallengesBox';

export default function SimpleContainer() {
    const [value, setValue] = React.useState<boolean>(true);

    const { userName } = useParams();
 
    const location = useLocation();
    const showList = location.pathname.includes('/list/profile');
    console.log(showList)
    return (

        <>
      <CssBaseline />
      <Container maxWidth="xl" className="container">
        <Box>
          <Grid container spacing={5}>
            <Grid item xs={12}>
              <Grid container justifyContent="center" spacing={10}>
                <Grid key={1} item>
                  <Paper sx={{ height: '100%', width: 350 }}>
                    <aside>
                      <Profile/>
                    </aside>
                  </Paper>
                </Grid>
                <Grid key={2} item>
                  <Paper sx={{ height: 'auto', width: 600 }}>
                    <main>
                    
                    {showList ? <MyList /> : <Posts param="/profile" />}
                    </main>
                  </Paper>
                </Grid>
                <Grid key={3} item>
                  <Paper sx={{ height: '100%', width: 350 }}>
                    <aside>
                     <Callendar
                     userName={userName}/>
                     <ChallengesBox
                     userName={userName} />
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