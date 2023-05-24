import IconButton from '@mui/material/IconButton/IconButton'
import Popover from '@mui/material/Popover/Popover'
import Typography from '@mui/material/Typography/Typography'
import React from 'react'
import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import {Requests} from '../FriendRequest/Requests';

export const NavBarRequests = () => {

    const [anchorE, setAnchorE] = React.useState<HTMLButtonElement | null>(null);
    const handleClic = (event: React.MouseEvent<HTMLButtonElement>) => {
      setAnchorE(event.currentTarget);
    };
  
    const handleClos = () => {
      setAnchorE(null);
    };
    const open2 = Boolean(anchorE);
    const id2 = open2 ? 'simple-popover' : undefined;



  return (
    <div> 
    <IconButton aria-describedby={id2}  onClick={handleClic} sx={{
      
    }}>
    <PeopleAltIcon sx={{
        color:'grey',
        display:'block'          
      }}/>

    </IconButton>
    <Popover
    disableScrollLock
      id={id2}
      open={open2}
      anchorEl={anchorE}
      onClose={handleClos}
      anchorOrigin={{
        vertical: 'bottom',
        horizontal: 'left',
      }}
      transformOrigin={{
        vertical: 'top',
        horizontal: 'center',
      }}
    ><p style={{margin:0}}>Pakvietimai</p>
      <Requests handleClos={handleClos}/>
    </Popover>
</div>
  )
}
