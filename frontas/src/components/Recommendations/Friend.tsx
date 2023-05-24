import React, { useEffect, useState } from "react";
import Avatar from "@mui/material/Avatar";
import { deepOrange, green } from "@mui/material/colors";
import PersonAddAlt1OutlinedIcon from "@mui/icons-material/PersonAddAlt1Outlined";
import PersonAddAlt1RoundedIcon from "@mui/icons-material/PersonAddAlt1Rounded";
import "../Users/User.css";
import Button from "@mui/material/Button/Button";
import axios from "axios";
import Box from "@mui/material/Box/Box";
import { Link, useNavigate } from "react-router-dom";
import { BorderBottom, BorderTop } from "@mui/icons-material";

interface IUser {
  username: any;
  name: any;
  surname: any;
  id: any;
  image64: string;
  index?: number;
}

const Friends: React.FC<IUser> = ({
  username,
  name,
  surname,
  image64,
  id,
  index,
}) => {
  const [roomId, setroomId] = useState<any>([]);
  const [pressed, setPressed] = useState(false);

  const token = `Bearer ${sessionStorage.getItem("token")}`;

  const handleClick = (Id: any) => {};
  const [selectedUserIds, setSelectedUserIds] = useState<string[]>([]);
  const handleUserSelection = (userId: string) => {
    // Check if the user ID is already in the array, if not add it to the array
    if (!selectedUserIds.includes(userId)) {
      setSelectedUserIds([...selectedUserIds, userId]);
    }
  };
  const navigate = useNavigate();
  const fetchComments = async () => {
    const { data } = await axios.get(
      `https://localhost:7019/api/Message/${id}`,
      {
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
          Authorization: token,
        },
      }
    );
    setroomId(data);
  };
  useEffect(() => {
    fetchComments();
  }, []);

  const handleUserClick = () => {
    setPressed(!pressed);

    const currentPathname = window.location.pathname;
    const lastPathSegment = currentPathname.split("/").pop();
    console.log(lastPathSegment);

    const numDashes = currentPathname
      .split("")
      .filter((segment) => segment === "-").length;
    const hasEightDashes = numDashes === 8;

    if (lastPathSegment === "recommendations") {
      if (hasEightDashes) {
        return;
      }
      navigate(`${id}`);
    } else if (lastPathSegment === id) {
      // Remove ID from URL
      const newPathname = currentPathname.replace(`/${id}`, "");
      navigate(newPathname);
    } else if (
      lastPathSegment !== "recommendations" &&
      currentPathname.includes(`${id}`)
    ) {
      navigate(`${currentPathname.replace(`/${id}`, "")}`);
    } else {
      if (hasEightDashes) {
        return;
      }
      navigate(`${id}`);
    }
  };

  var boxSx = {
    bgcolor: pressed ? "green" : "transparent",
    "&:hover": {
      opacity: [0.9, 0.8, 0.7],
      cursor: "pointer",
    },
  };

  var firstBoxSx = {
    ...boxSx,
    borderTop: "solid",
    borderBottom: "solid",
  };

  var otherBoxSx = {
    ...boxSx,
    borderBottom: "solid",
  };

  return (
    <Box onClick={handleUserClick} sx={index === 0 ? firstBoxSx : otherBoxSx}>
      <div style={{ display: "flex" }}>
        <Avatar
          src={`data:image/jpeg;base64,${image64}`}
          sx={{ margin: 2 }}
          variant="rounded"
        />
        <div className="user">
          <p className="username">{username}</p>
          <p className="name">
            {name} {surname}
          </p>
        </div>
      </div>
    </Box>
  );
};

export default Friends;
