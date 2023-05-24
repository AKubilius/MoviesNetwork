import React, { useEffect, useState } from 'react'
import './Collection.css'
import { Box, Button, Link } from '@mui/material'
import AddCircleIcon from '@mui/icons-material/AddCircle';
import axios from 'axios';
import CreatePost from './CreatePost';
import Dialog from '@mui/material/Dialog';
import DialogTitle from '@mui/material/DialogTitle';
import DialogContent from '@mui/material/DialogContent';
import DialogActions from '@mui/material/DialogActions';
import SendToFriend from './SendToFriend';


interface IPost {
    id: any;
    poster_path: any;
    createdDate: any;
    title: any;
    backdrop_path: any;
    friends: any[] | null;
    rating: any;
}

const Movie: React.FC<IPost> = ({
    id,
    poster_path,
    backdrop_path,
    createdDate,
    title,
    friends,
    rating,


}) => {
    const [currentMovieDetail, setMovie] = useState<any>([]);

    useEffect(() => {
        getData()
        window.scrollTo(0, 0)
    }, [])
    const getData = () => {
        fetch(`https://api.themoviedb.org/3/movie/${id}?api_key=c9154564bc2ba422e5e0dede6af7f89b&language=lt-LT`)
            .then(res => res.json())
            .then(data => setMovie(data))
    }
    return (
        <div className='collection'>
            <Box >
                <Box sx={{ display: 'flex', borderRadius: 2, width: '100%', border: 1 }}>
                    <div className='movies'>
                        <Box
                            component="img"
                            sx={{
                                margin: 1,
                                borderRadius: 2,
                                height: '90%',
                                width: '20%',
                            }}
                            alt="Movie"
                            src={`https://image.tmdb.org/t/p/original${poster_path}`}
                        />
                        <Box sx={{ display: 'flex', flexDirection: 'column', borderRadius: 2, width: '100%', marginTop: 1, height: 198, justifyContent: 'space-between' }}>

                            <Box sx={{
                                display: 'flex',
                                flexDirection: 'row',
                                justifyContent: 'space-between'
                            }}>
                                <Box>
                                    <Link href={`/movie/${id}`}>{title}</Link>
                                    <h4>{createdDate.slice(0, -6)}</h4>
                                </Box>
                                <h3 style={{
                                    marginTop: 0,
                                    marginRight: 20
                                }}>{rating}</h3>
                            </Box>
                            <Box sx={{
                                display: 'flex',
                                flexDirection: 'column',
                                justifyContent: 'space-between'
                            }}>
                                <div className="genres">
                                    {
                                        currentMovieDetail && currentMovieDetail.genres
                                            ?
                                            currentMovieDetail.genres.map((genre: { id: string | undefined; name: string | number | boolean | React.ReactElement<any, string | React.JSXElementConstructor<any>> | React.ReactFragment | React.ReactPortal | null | undefined; }) => (
                                                <><span className="genre" id={genre.id}>{genre.name}</span></>
                                            ))
                                            : ""}
                                </div>
                                <Box sx={{ borderRadius: 2, marginBottom: 0, display: 'flex', flexDirection: 'flex', alignItems: 'end', justifyContent: 'left' }}>
                                    {friends ? <div style={{ display: 'flex' }}>
                                        <SendToFriend
                                            friends={friends}
                                            movieId={id}
                                            imgUrl={backdrop_path}
                                            body={title}
                                        />
                                        <CreatePost
                                            movieId={id}
                                            imgUrl={backdrop_path}
                                            body={title}
                                        />
                                    </div> : ''
                                    }
                                </Box>
                            </Box>
                        </Box>
                    </div>
                </Box>
            </Box>
        </div>
    )
}
export default Movie
