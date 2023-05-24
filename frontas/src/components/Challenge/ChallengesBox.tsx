import { Box } from '@mui/material'
import React, { useEffect, useState } from 'react'
import { getRequest } from "../Api/Api";
import Challenge from './Challenge';
import ChallengesModal from './ChallengesModal';
import CustomModalWithTabs from './ChallengeTest';
import ChallengesTest from './ChallengeTest';
import ChallengesAllModal from './ChallengesAllModal';

interface Challenges {
  name: string;
  count: number;
}
interface userChallenge {
  progress: number;
  completed: boolean;
  challenge: Challenges;
}
interface ChallengesBoxProps {
  userName: string | undefined;
}

const boxStyles = {
  display: 'flex',
  flexDirection: 'column',
  alignContent: 'center',
  justifyContent: 'center'
}
const h4Styles = {
  justifyContent: 'center',
  display: 'flex',
  marginBottom: 0
}


const ChallengesBox: React.FC<ChallengesBoxProps> = ({ userName }) => {


  const [challenges, setChallenges] = useState([]);
  const [allChallenges, setAllChallenges] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      const data = await getRequest('https://localhost:7019/api/Challenge/MyChallenges/', userName ? userName : '');
      setChallenges(data);
    };
    fetchData();
  }, []);

  useEffect(() => {
    const fetchData = async () => {
      const data = await getRequest('https://localhost:7019/api/Challenge/notJoined', '');
      setAllChallenges(data);
    };
    fetchData();
  }, []);
  return (
    <Box sx={boxStyles}>
      <h4 style={h4Styles}>Iššūkiai</h4>

      {challenges?.filter((challenge: userChallenge) => !challenge.completed).slice(0, 3).map((challenge: userChallenge) => (
        <Challenge
          userChallenge={challenge}
        />
      ))}
      <Box sx={{
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'space-between'
      }}>
        {
          userName ? '' : <ChallengesAllModal
            userChallenge={allChallenges}
          />
        }     <ChallengesModal
          userChallenge={challenges}
        />
      </Box>

    </Box>
  )
}

export default ChallengesBox