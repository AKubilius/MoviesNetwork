import Avatar from '@mui/material/Avatar/Avatar'
import Box from '@mui/material/Box/Box'
import Users from './Users'
import { Link } from '@mui/material';
import axios from 'axios';
import { useEffect, useState } from 'react';

const UserSide = () => {
  const avatarSx = { margin: 2 };
  const boxSx = { borderBottom:1, display: 'flex'}
  const [image,setImage] = useState<any>([]);

  const token = `Bearer ${sessionStorage.getItem("token")}`
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

  return (
    <div className='userSide'>
        <Box sx={boxSx}>
            <Avatar src={`data:image/jpeg;base64,${image}`} sx={avatarSx} variant="rounded" />
            <h4>
              <Link href={'/profile'} style={{textDecoration:'none'}}>{sessionStorage.getItem("name")}</Link>
            </h4>
        </Box>

        
           <Users/>
    </div>
  )
}

export default UserSide