// webpack.config.js
const path = require('path');

module.exports = {
  mode: "production",
  entry: {
    'shared': {
      import: [
        'react',
        'react-dom',
        'styled-components',
        'react-hook-form',
      ]
    },
    index: {
      dependOn: 'shared',
      import: './src/Index.tsx',
    },
  },
  output: {
    path: path.resolve(__dirname, '../static'),
    filename: '[name].js',
  },
  module: {
    rules: [
      {
        test: /\.(js|jsx|ts|tsx)$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: [
                "@babel/preset-env",
                ['@babel/preset-react', { "runtime": "automatic" }],
                "@babel/preset-typescript"
            ],
            plugins: [
                //
            ],
          },
        },
      },
    ],
  },
  resolve: {
    extensions: ['.js', '.jsx','.ts','.tsx'],
  },
};
