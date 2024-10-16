import { createRoot } from 'react-dom/client';
import React from 'react';

interface OnLoad {
  (): void
}

// OnLoad handler to render the app
export function makeOnLoad(C: React.ComponentType<any>): OnLoad {
  return () => {
    const app = document.getElementById('app');
    if (app) {
      const root = createRoot(app);
      root.render(<C />);
    }
  };
} 
