import React, { useState } from 'react';
import {
  deleteNewsImage,
  updateNewsImage,
} from '../../api/newsApi';
import { useNavigate } from 'react-router-dom';
import { DetailNews } from '../../types';

interface NewsImageUploaderProps {
  newsId: string;
  currentImageUrl?: string | null;
  onImageChange: (updatedNews: DetailNews) => void;
}

const NewsImageUploader: React.FC<NewsImageUploaderProps> = ({
  newsId,
  currentImageUrl,
  onImageChange,
}) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleFileChange = async (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setLoading(true);
    setError(null);

    try {
      const updatedNews = await updateNewsImage(newsId, file);
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
      const updatedNews = await deleteNewsImage(newsId);
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

export default NewsImageUploader;