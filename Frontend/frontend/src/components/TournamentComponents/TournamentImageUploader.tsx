import React, { useState } from 'react';
import { Button, Image, Spinner, Alert, Form } from 'react-bootstrap';
import { updateTournamentImage, deleteTournamentImage } from '../../api/tournamentApi';
import { TournamentDto } from '../../types';

interface TournamentImageUploaderProps {
  tournamentId: string;
  currentImageUrl?: string | null;
  onImageChange: (updatedTournament: TournamentDto) => void;
}

const TournamentImageUploader: React.FC<TournamentImageUploaderProps> = ({
  tournamentId,
  currentImageUrl,
  onImageChange,
}) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setLoading(true);
    setError(null);

    try {
      const updatedTournament = await updateTournamentImage(tournamentId, file);
      onImageChange(updatedTournament);
    } catch (err: any) {
      setError(err.message || 'Не удалось обновить изображение');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Вы уверены, что хотите удалить изображение?')) return;
    
    setLoading(true);
    setError(null);

    try {
      const updatedTournament = await deleteTournamentImage(tournamentId);
      onImageChange(updatedTournament);
    } catch (err: any) {
      setError(err.message || 'Не удалось удалить изображение');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="text-center">
      {currentImageUrl && (
        <Image
          src={currentImageUrl}
          alt="Изображение турнира"
          fluid
          rounded
          className="mb-3"
          style={{ maxHeight: '200px' }}
        />
      )}

      <Form.Group controlId="formFile" className="mb-3">
        <Form.Label>{currentImageUrl ? 'Заменить изображение' : 'Добавить изображение'}</Form.Label>
        <Form.Control 
          type="file" 
          accept="image/*" 
          onChange={handleFileChange} 
          disabled={loading}
        />
      </Form.Group>

      {currentImageUrl && (
        <Button
          variant="danger"
          onClick={handleDelete}
          disabled={loading}
          className="mb-3"
        >
          {loading ? <Spinner size="sm" /> : 'Удалить изображение'}
        </Button>
      )}

      {loading && <Spinner animation="border" size="sm" />}
      {error && <Alert variant="danger" className="mt-2">{error}</Alert>}
    </div>
  );
};

export default TournamentImageUploader;