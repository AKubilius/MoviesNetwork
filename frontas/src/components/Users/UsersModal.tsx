import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import User from './User';

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

export default function UsersModal({ users }: any) {
  const [open, setOpen] = React.useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);

  return (
    <div style={{ display: 'flex', justifyContent: 'end' }}>
      <Button onClick={handleOpen}>Daugiau narių</Button>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
          {users?.map((user: any, index: React.Key | null | undefined) => (
            <User
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