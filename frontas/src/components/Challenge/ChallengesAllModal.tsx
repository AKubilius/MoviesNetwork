import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import Challenge from './Challenge';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import AllChallenge from './AllChallenge';
import ChallengeForm from '../Form/ChallengeForm';
import { makePostRequest } from '../Api/Api';

const style = {
  position: 'absolute' as 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  p: 4,
};

const newModalStyle = {
  ...style,
  width: 600,
};

interface Challenges {
  name: string;
  count: number;
  id:number;
}
interface userChallenge {
  progress: number;
  completed: boolean;
  challenge: Challenges;
}

interface UserChallengeProps {
  userChallenge: userChallenge[];
}

export default function ChallengesAllModal({ userChallenge }: UserChallengeProps) {
  const [open, setOpen] = React.useState(false);
  const [newModalOpen, setNewModalOpen] = React.useState(false);

  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);

  const handleNewModalOpen = () => setNewModalOpen(true);
  const handleNewModalClose = () => setNewModalOpen(false);

  const admin =sessionStorage.getItem("admin")


  return (
    <div style={{ display: 'flex', justifyContent: 'end' }}>
      <Button onClick={handleOpen}>Visi iššūkiai</Button>
      <Modal open={open} onClose={handleClose} aria-labelledby="modal-modal-title" aria-describedby="modal-modal-description">
        <Box sx={style}>
          <Typography id="modal-modal-title" variant="h6" component="h2">
            {userChallenge.map((challenge: any) => (
              <AllChallenge userChallenge={challenge} />
            ))}
          </Typography>
         
         {
            admin === 'false' ? '' : <Button startIcon={<AddCircleIcon />} variant="text" color={'primary'} onClick={handleNewModalOpen}>
            {'Pridėti'}
          </Button>
         }
          
        </Box>
      </Modal>


      <Modal open={newModalOpen} onClose={handleNewModalClose} aria-labelledby="new-modal-title" aria-describedby="new-modal-description">
        <Box sx={newModalStyle}>
          <ChallengeForm/>
          
        </Box>
      </Modal>
    </div>
  );
}