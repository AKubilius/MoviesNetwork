import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import Container from '@mui/material/Container';
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
import { useEffect, useRef, useState } from 'react';
import FriendsChat from '../FriendsChat/FriendsChat';
import Chat from '../Chat/Chat';
import TextField from '@mui/material/TextField/TextField';
import SendIcon from '@mui/icons-material/Send';
import SendMessage from '../Chat/SendMessage';
import * as signalR from '@microsoft/signalr';

interface Movie {
    id: number;
    title: string;
    overview: string;
    poster_path: string;
}

const paperStyle = {
    position: 'relative',
    height: 830,
    width: 600,
    backgroundColor: '#E8F4FD',

    overflowY: 'auto',
    '&::-webkit-scrollbar': {
        width: '20px',
    },
    '&::-webkit-scrollbar-thumb': {
        backgroundColor: 'rgba(0, 0, 0, 0.4)',
        borderRadius: '4px',
    },
    '&::-webkit-scrollbar-track': {
        backgroundColor: 'rgba(0, 0, 0, 0.1)',
        borderRadius: '4px',
    },

    // Add this line to make it scrollable
}

const ChatContainer: React.FC = () => {
    const [movies, setMovies] = useState<Movie[]>([]);
    const Url = "https://image.tmdb.org/t/p/original"
    const handleResults = (results: Movie[]) => {
        setMovies(results);
    };
    const [input, setInput] = useState('');
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);


    const { id: roomID } = useParams<{ id: string | undefined }>();

    const userName = `${sessionStorage.getItem("name")}`;
    const sendMessage = async () => {
        if (input && connection && roomID && connection.state === signalR.HubConnectionState.Connected) {
            await connection.send('SendMessageToRoom', roomID, userName, input);
            setInput('');
        }
    };


    return (

        <>
            <CssBaseline />
            <Container maxWidth="xl" className='container'  >
                <Box >
                    <Grid container spacing={2} sx={{ height: '80%' }}>
                        <Grid item xs={12} >
                            <Grid container justifyContent="center" spacing={1}>
                                <Grid key={1} item>
                                    <Paper

                                        sx={{
                                            height: '100%',
                                            position: 'relative',
                                            width: 350,

                                            backgroundColor: (theme) =>
                                                theme.palette.mode === 'dark' ? '#1A2027' : '#fff',
                                        }}>
                                        <aside>
                                            <FriendsChat />
                                        </aside>
                                    </Paper>
                                </Grid>
                                <Grid key={2} item  >
                                    <Paper
                                    
                                        sx={paperStyle}>
                                        <main style={{ width: "100%", paddingBottom: "1rem", marginTop:20 }}>
                                           
                                            <Chat
                                                connection={connection}
                                                setConnection={setConnection}
                                                roomID={roomID}

                                            />
                                        </main>

                                    </Paper>
                                    <SendMessage
                                        input={input}
                                        setInput={setInput}
                                        connection={connection}
                                        roomID={roomID}
                                        sendMessage={sendMessage}
                                    />
                                </Grid>
                                <Grid key={3} item>
                                    <Paper
                                        sx={{
                                            height: '100%',
                                            width: 350,
                                            backgroundColor: (theme) =>
                                                theme.palette.mode === 'dark' ? '#1A2027' : '#fff'
                                        }}>
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
export default ChatContainer;