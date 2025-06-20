import ErrorBoundary from '../../components/ErrorBoundary';
import MatchesList from '../../components/MatchComponents/MatchesList';

const MatchesPage: React.FC = () => {
  return (
    <div className="flex-grow-1">
      <h2>Matches</h2>
      <ErrorBoundary>
      <MatchesList />
      </ErrorBoundary>
    </div>
  );
};

export default MatchesPage;