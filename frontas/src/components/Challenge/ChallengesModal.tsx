import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import Challenge from './Challenge';
import AddCircleIcon from '@mui/icons-material/AddCircle';

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

interface Challenges
{
  name:string;
  count:number;
  id:number;
}
interface userChallenge {
  progress:number;
  completed:boolean;
  challenge:Challenges;
}

interface UserChallengeProps {
    userChallenge: userChallenge[];
  }

  const h4Style = {display:'flex', justifyContent:'center'}
  const line = { borderTop: '1px solid #ccc', margin: '16px 0' }

  export default function ChallengesModal( { userChallenge }: UserChallengeProps ) {
  const [open, setOpen] = React.useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);

  return (
    <div style={{display:'flex',justifyContent:'end'}}>
      <Button onClick={handleOpen}>Naudotojo iššūkiai</Button>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
          <Typography id="modal-modal-title" variant="h6" component="h2">
          {userChallenge
?.filter((challenge: userChallenge) => !challenge.completed)
  .map((challenge: userChallenge) => (
    <Challenge userChallenge={challenge} />
  ))}

<h3 style={h4Style}>Įvykdyti iššūkiai</h3>
<div style={line}></div>

{userChallenge
  ?.filter((challenge: userChallenge) => challenge.completed)
  .map((challenge: userChallenge) => (
    <Challenge userChallenge={challenge} />
  ))}
          </Typography>
        </Box>
      </Modal>
    </div>
  );
}