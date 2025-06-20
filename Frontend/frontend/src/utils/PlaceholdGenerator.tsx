
export const generateSVGPlaceholder = (width: number, height: number, text: string = "No Image") => {
  const svg = `
    <svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">
      <rect width="100%" height="100%" fill="#f0f0f0"/>
      <text x="50%" y="50%" fill="#666" font-family="Arial" font-size="12" text-anchor="middle" dominant-baseline="middle">${text}</text>
    </svg>
  `;
  return `data:image/svg+xml;charset=UTF-8,${encodeURIComponent(svg)}`;
};
