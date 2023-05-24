import React, { useEffect, useState } from 'react'
import Box from '@mui/material/Box';
import Avatar from '@mui/material/Avatar';
import { blueGrey, red} from '@mui/material/colors';
import Button from '@mui/material/Button';

import './Post.css'
import axios from 'axios';
import ThumbUpIcon from '@mui/icons-material/ThumbUp';
import ThumbUpOutlinedIcon from '@mui/icons-material/ThumbUpOutlined';
import TextField from '@mui/material/TextField/TextField';
import Comments  from './Comment';
import ReplyIcon from '@mui/icons-material/Reply';
import DeleteIcon from '@mui/icons-material/Delete';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import {makePostRequest, makeDeleteRequest} from "../Api/Api";

interface IPost {
  id: any;
  body: any;
  createdDate: any;
  imageUrl: any;
  movieId: any;
  image64:string;
}

const Post: React.FC<IPost> = ({
  id,
  body,
  createdDate,
  imageUrl,
  movieId,
  image64
}) => {

  const [pressed, setPressed] = useState(true);
  const [pressedLike, setPressedLike] = useState(true);
  const [inList, setInList] = useState(false);
  const [likes, setLikes] = useState(0);
  const [comments,setComments] = useState<any>([]);
  const [image,setImage] = useState<any>([]);

  const token = `Bearer ${sessionStorage.getItem("token")}`

  const handleLike = (Id: any) => {
    setPressedLike(!pressedLike);
    pressedLike ? unLikePost(Id) : likePost(Id)
    pressedLike ? setLikes(likes - 1) : setLikes(likes + 1)
  };

  const handleClick = (Id: any) => {
   
    inList ?  deleteFromList(Id) :createRequest(Id)
    setInList(!inList);
  };

  const [value, setValue] = useState('');

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.keyCode === 13) {
      event.preventDefault();
      handleSubmit();
    }
  };

  const handleSubmit = ( ) => {
    // Handle form submission
    
    commentPost(value)
    setValue('');
  };

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setValue(event.target.value);
  };

  async function commentPost(body: string) {
    const { data, status } = await makePostRequest('https://localhost:7019/api/Comment', { Body: body, postId: id });
    return data;
  }

  const fetchComments = async () => {

    const { data } = await axios.get(`https://localhost:7019/api/Comment/${id}`,
      {
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
          Authorization: token
        },
      })
    setComments(data)
  }
  useEffect(() => {
    fetchComments()
  }, [])


  const getUserImage = async () => {
    const { data } = await axios.get(`https://localhost:7019/User/currentImage`,
      {
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
          Authorization: token
        },
      })
    setImage(data)
  }
  useEffect(() => {
    getUserImage()
  }, [])


  const fetchLikes = async () => {

    const { data } = await axios.get(`https://localhost:7019/like/${id}`,
      {
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
          Authorization: token
        },
      })
    setLikes(data)


  }
  useEffect(() => {
    fetchLikes()
  }, [])

  const fetchIsLiked = async () => {
    const { data } = await axios.get(`https://localhost:7019/like/isliked/${id}`,
      {
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
          Authorization: token
        },
      })

    setPressedLike(data)
  }
  useEffect(() => {
    fetchIsLiked()
  }, [])


  const fetchIsInList = async () => {
    const { data } = await axios.get(`https://localhost:7019/list/isListed/${id}`,
      {
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
          Authorization: token
        },
      })

      setInList(data)
  }
  useEffect(() => {
    fetchIsInList()
  }, [])



  async function likePost(Id: any) {
    const { data } = await makePostRequest('https://localhost:7019/Like', { PostId: Id.id });

    return data;
  }
  
  async function unLikePost(Id: any) {
    const { data } = await makeDeleteRequest(`https://localhost:7019/Like/${Id.id}`);

    return data;
  }

  async function createRequest(Id: any) {
    const { data } = await makePostRequest('https://localhost:7019/List/Mylist', { MovieID: `${Id.movieId}` });
    return data;
  }

  async function deleteFromList(Id: any) {
    const { data } = await makeDeleteRequest(`https://localhost:7019/List/${Id.movieId}`);

    return data;
  }

  const buttonSx = 
  {
    borderRadius: 5,
    margin: 1
  }

  const neutralButtonSx = {
    ...buttonSx,
  };

  const deleteButtonSx = {
    ...buttonSx,
  };
  

  return (
    <div className='post'>
      <Box sx={{ borderRadius: 5, border: '2px solid' }}>
        <Box sx={{ display: 'flex' }}>
          <Avatar   src={`data:image/jpeg;base64,${image64}`}  sx={{ bgcolor: blueGrey[900], margin: 2 }} variant="rounded" />
          <h3>{body}</h3>
        </Box>
        
        <Box
          component="img"
          alt="Movie"
          sx={{ marginLeft: 1, marginRight: 1, borderRadius: 5, width: '97%', }}
          src={`https://image.tmdb.org/t/p/original${imageUrl}`}
        />
        <Box>

          <Box sx={{ marginLeft: 5, marginRight: 5, borderTop:1, borderBottom:1, borderColor: 'grey', display: 'flex', alignContent: 'center', justifyContent:'space-between' }}>
            <div style={{display:'flex'}}>
            <p> {likes} </p>
            <Button onClick={() => handleLike({ id })} sx={neutralButtonSx} startIcon={pressedLike ? <ThumbUpIcon /> : <ThumbUpOutlinedIcon />} variant="text">Patinka</Button>
            </div>
            <Button 
              onClick={() => handleClick({ movieId })} 
              sx={inList ? deleteButtonSx : neutralButtonSx } 
              startIcon={inList ? <DeleteIcon/>   :  <AddCircleIcon />} 
              variant="text"
              color={inList ? 'secondary' : 'primary' } 
            >
              {inList ? "Pašalinti" : "Pridėti" }
            </Button>

          </Box>
          
          <Box component="form" noValidate autoComplete="off" sx={{ display:'flex', flexDirection:'row', alignContent:'center'}}>
            <div style={{display:'grid',alignContent:'center', margin:10}}>
              <Avatar 
                sx={{ bgcolor: blueGrey[900] }} 
                src={`data:image/jpeg;base64,${image}`}
              />
            </div>

            <TextField 
              fullWidth 
              id="outlined-multiline-static" 
              label="Komentaras" 
              sx={{marginRight:1, marginBottom:2, marginTop:2, overflow:'hidden'}}
              inputProps={{ style: { overflow: 'hidden' } }}
              value={value}
              onChange={handleChange}
              onKeyDown={handleKeyDown}
            />
          </Box>
          
          <Box sx={{ display:'flex', flexDirection:'column' }}>
            { comments ? comments?.slice(0,3).map((user: any, index: React.Key | null | undefined) => (
                <Comments
                    body={user.body}
                    img={user.user.profileImageBase64}
                    id ={user.id}
                    user={user.user}
                    key = {user.id}
                />
            )): null}
          </Box>
        </Box>
      </Box>
    </div>
  )
}

export default Post
