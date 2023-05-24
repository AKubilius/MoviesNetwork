import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import User from './User';
import { useState } from 'react';
import axios from 'axios';
import Friends from './Friends';

const style = {
  position: 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  p: 4,
};

export default function FriendsModal() {
  const [open, setOpen] = React.useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);

  const [friends, setFriends] = useState<any>([]);
  const token = `Bearer ${sessionStorage.getItem("token")}`;
  const fetch = async () => {
    const { data } = await axios.get(`https://localhost:7019/user/friends`, {
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
        Authorization: token,
      },
    });
    setFriends(data);
    console.log(data);
  };
  React.useEffect(() => {
    fetch();
  }, []);

  return (
    <div style={{ display: 'flex', justifyContent: 'end' }}>
      <Button onClick={handleOpen}>Draugai</Button>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
          {friends?.map((user: any, index: React.Key | null | undefined) => (
            <Friends
              username={user.userName}
              name={user.name}
              surname={user.surname}
              image64={user.profileImageBase64}
              id={user.id}
              key={user.id}
            />
          ))}
        </Box>
      </Modal>
    </div>
  );
}