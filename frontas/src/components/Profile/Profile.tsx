import Avatar from '@mui/material/Avatar/Avatar'
import Box from '@mui/material/Box/Box'
import green from '@mui/material/colors/green'
import React, { useEffect, useState } from 'react'
import { Link as RouterLink , useParams  } from 'react-router-dom';
import Link from '@mui/material/Link';
import LinearProgress, { LinearProgressProps } from '@mui/material/LinearProgress';
import axios from 'axios'
import ImageUpload from '../Form/UploadImageForm';
import ImageForm from '../Form/ImageForm';

const style = {
  display: 'flex',
  flexDirection: 'column',
  width: '100%',
  marginTop: 1,
  '&:hover': {

    opacity: [0.9, 0.8, 0.7],
  },
};


interface User {
  userName: any;
  name: any;
  surname: any;
  id: any;
  profileImageBase64: string;
}
export const Profile = () => {
  const [progress, setProgress] = useState(0);

  const token = `Bearer ${sessionStorage.getItem("token")}`
  const { userName } = useParams(); // Get the username parameter from the URL
  
  const [currentUser, setCurrentUser] = useState<User | null>(null);

  const Image_Path = "https://image.tmdb.org/t/p/w500"

  const API = 'https://api.themoviedb.org/3/'
  const[movies,setMovies] = useState<any[]>([])
  const[List,setList] = useState<any[]>([])

  const getCompatibility = async () => {
    try {
      const { data: { compatibility } } = await axios.get(`https://localhost:7019/list/compatibility/${userName}`,
        {
          headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json',
            Authorization: token
          },
        })
      setProgress(compatibility)
    }
    catch (error) {
      console.error('Bandant gaut Compability gautas error : ', error)
    }
  }
  useEffect(() => {
    if (!userName) {
      getLoggedInUser()
    }
    else {
      getCompatibility()
      getUserProfile()
    }
  }, [])

  const getLoggedInUser = async () => {
    try {
      const { data } = await axios.get(`https://localhost:7019/user/current`,
        {
          headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json',
            Authorization: token
          },
        })
      setCurrentUser(data);
    }

    catch (error) {
      console.error('Bandant gaut Compability gautas error : ', error)
    }
  }
  const getUserProfile = async () => {
    try {
      const { data } = await axios.get(`https://localhost:7019/user/${userName}`,
        {
          headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json',
            Authorization: token
          },
        })
      setCurrentUser(data);
    }

    catch (error) {
      console.error('Bandant gaut Compability gautas error : ', error)
    }
  }
  


  const getProgressColor = () => {
    if (progress <= 30) {
      return 'red';
    } else if (progress <= 60) {
      return 'blue';
    } else {
      return 'green';
    }
  };


  return (
    <div className='userInfo'>
      {progress === 0 && (<ImageForm/>)}
      

{currentUser ? (
      <Box sx={{ display: 'flex', borderRadius: 5, marginBottom: 15, border: 'solid', justifyContent:'flex-start' }}>
        <Avatar src={`data:image/jpeg;base64,${currentUser.profileImageBase64}`} sx={{ marginBottom: 5,marginTop: 5,marginLeft: 5,marginRight:1, height: 100, width: 100 }} variant="rounded" />
        <Box sx={{
          display:'flex',
          flexDirection:'column',
          justifyContent:'center'
        }}>
        <Box >
        <p style={{ fontSize: 25,marginBottom:0 }}>{currentUser.userName}</p>
        </Box>
        <p style={{ fontSize: 15 }}>{currentUser.name} {currentUser.surname}</p>
      </Box>
        </Box>
        
 ) : (
  <div>Loading...</div>
)} 
      {progress !== 0 && (
        
        <div>
          <h4 style={{
  display:'flex',
  justifyContent:'center'
}}>Suderinamumo įvertis</h4>
          <LinearProgress
            variant="determinate"
            value={progress}
            sx={{
              flexGrow: 1,
              backgroundColor: 'rgba(0, 0, 0, 0.12)',
              '& .MuiLinearProgress-bar': {
                backgroundColor: getProgressColor(),
              },
            }}
          />
          <span style={{ marginLeft: '1rem' }}>{progress.toFixed(2)}%</span>
        </div>
      )}

      <Box>
        <Box sx={{
          display: 'flex',
          flexDirection: 'column',
        }}>
          <Box
            sx={style}>
            <Link  to={`/profile${userName? `/${userName}` : ``}` } component={RouterLink} >Įrašai</Link>
          </Box>
          <Box sx={style}>
            <Link to={`/list/profile${userName? `/${userName}` : ``}`} component={RouterLink}>Sąrašas</Link>
          </Box>
        </Box>
      </Box>
    </div>
  )
}


