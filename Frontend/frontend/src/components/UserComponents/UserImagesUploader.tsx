import React, { useState } from 'react';
import {
  deleteProfileImage,
  updateProfileImage,
} from '../../api/userApi';
import { useNavigate } from 'react-router-dom';
import { UserProfileDto } from '../../types';

interface NewsImageUploaderProps {
  profileId: string;
  currentImageUrl?: string | null;
  onImageChange: (updatedNews: UserProfileDto) => void;
}

const ProfileImageUploader: React.FC<NewsImageUploaderProps> = ({
  profileId,
  currentImageUrl,
  onImageChange,
}) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleFileChange = async (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setLoading(true);
    setError(null);

    try {
      const updatedNews = await updateProfileImage(profileId, file);
      onImageChange(updatedNews);
    } catch (err: any) {
      setError(err.message || 'Не удалось обновить изображение');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    setLoading(true);
    setError(null);

    try {
      const updatedNews = await deleteProfileImage(profileId);
      onImageChange(updatedNews);
    } catch (err: any) {
      setError(err.message || 'Не удалось удалить изображение');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ textAlign: 'center' }}>
      {currentImageUrl && (
        <img
          src={currentImageUrl}
          alt="Превью"
          style={{ width: '300px', marginBottom: '10px' }}
        />
      )}

      <div>
        <input type="file" accept="image/*" onChange={handleFileChange} disabled={loading} />
      </div>

      {currentImageUrl && (
        <button onClick={handleDelete} disabled={loading}>
          {loading ? 'Удаление...' : 'Удалить изображение'}
        </button>
      )}

      {loading && <p>Загрузка...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}
    </div>
  );
};

export default ProfileImageUploader;