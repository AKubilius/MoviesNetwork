import Button from '@mui/material/Button/Button';
import Input from '@mui/material/Input/Input';
import React, { useState } from 'react';

const ImageUpload = () => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const token = `Bearer ${sessionStorage.getItem("token")}`
  const onFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files[0]) {
      setSelectedFile(event.target.files[0]);
    }
  };

  const onFileUpload = async () => {
    if (!selectedFile) {
      alert('Please select a file');
      return;
    }

    const formData = new FormData();
    formData.append('imageFile', selectedFile);

    try {
        const response = await fetch('https://localhost:7019/user/upload-image', {
          method: 'POST',
          headers: {
            'Authorization': token,
          },
          body: formData,
        });

      if (response.ok) {
        alert('Image uploaded successfully');
      } else {
        alert('Failed to upload image');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Failed to upload image');
    }
  };

  return (
    <div>
       <Input type="file" onChange={onFileChange} />
      <Button onClick={onFileUpload}>Keisti nuotrauką</Button>
    </div>
  );
};

export default ImageUpload;