import Avatar from '@mui/material/Avatar/Avatar';
import Box from '@mui/material/Box/Box';
import Typography from '@mui/material/Typography/Typography';
import { blueGrey } from '@mui/material/colors';
import React from 'react'

interface Comment {
  body: any;
  img: any;
  id: any;
  user: User;
}
interface User {
  userName: string;
}

const Comments: React.FC<Comment> = ({
  body,
  img,
  id,
  user
}) => {

  return (
    <Box sx={{ display: 'flex', }}>
       <Box
      sx={{
        display: 'flex',
        flexDirection: 'row',
        alignItems: 'center',
        padding: '8px',
        borderBottom: '1px solid rgba(0, 0, 0, 0.12)',
      }}
    >
      <Avatar
        sx={{
          bgcolor: blueGrey[900],
          marginRight: '8px',
          height:33,
          width:33
        }}
        src={`data:image/jpeg;base64,${img}`}
      />
      <Box>
        <Typography variant="subtitle2" sx={{ fontWeight: 'bold' }}>
        {user.userName}
        </Typography>
        <Typography variant="body2" sx={{ marginTop: '4px' }}>
        {body}
        </Typography>
      </Box>
    </Box>
    </Box>
  )
}
export default Comments