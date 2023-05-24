import Avatar from '@mui/material/Avatar/Avatar';
import IconButton from '@mui/material/IconButton/IconButton';
import Popover from '@mui/material/Popover/Popover';
import Typography from '@mui/material/Typography/Typography';
import React from 'react'
import NotificationsIcon from '@mui/icons-material/Notifications';

export const NavBarNotifications = () => {
    const [anchorEl, setAnchorEl] = React.useState<HTMLButtonElement | null>(null);

    const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };


  const open = Boolean(anchorEl);
  
  const id = open ? 'simple-popover' : undefined;
  return (
    <div>
        <IconButton aria-describedby={id}  onClick={handleClick} sx={{
        
    }}>
    

    </IconButton>
    <Popover
    disableScrollLock
      id={id}
      open={open}
      anchorEl={anchorEl}
      onClose={handleClose}
      anchorOrigin={{
        vertical: 'bottom',
        horizontal: 'left',
      }}
    >
      <Typography sx={{ p: 2 }}>Notification Window</Typography>
    </Popover>
    
      
    </div>
  )
}
