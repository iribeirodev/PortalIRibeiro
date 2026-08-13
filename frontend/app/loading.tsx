export default function Loading() {
  return (
    <div className="loading-container">
      <div className="spinner-box">
        <div className="spinner-ring"></div>
        <div className="spinner-core"></div>
      </div>
      <div className="loading-text">Carregando o Portal...</div>
    </div>
  );
}
