import Box from "@mui/material/Box/Box";
import "./Filter.css";
import { Link } from "@mui/material";

export default function FilterOption(option: any) {
  return (
    <div className="filterOption">
      <Box
        sx={{
          marginTop: 1,
          "&:hover": {
            opacity: [0.9, 0.8, 0.7],
          },
        }}
      >
        <Link sx={{ marginLeft: 1, textDecoration: 'none' }} href={`/movies/${option.id}`}>
          {option.name}
        </Link>
      </Box>
    </div>
  );
}
