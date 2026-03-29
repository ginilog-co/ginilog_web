const fs = require('fs');
const path = require('path');

// Delete new files in app/
const itemsToDelete = ['about', 'admin', 'contact', 'portal', 'privacy', 'services', 'terms', 'page.tsx'];
itemsToDelete.forEach(item => {
  const fullPath = path.join('app', item);
  try {
    const stat = fs.statSync(fullPath);
    if (stat.isDirectory()) {
      fs.rmSync(fullPath, { recursive: true, force: true });
    } else {
      fs.unlinkSync(fullPath);
    }
    console.log('Deleted:', item);
  } catch (e) {
    console.log('Not found:', item);
  }
});

// Move existing folders to app/
const foldersToMove = ['landing-page', 'admin-dashboard', 'customer-portal'];
foldersToMove.forEach(folder => {
  try {
    fs.renameSync(folder, path.join('app', folder));
    console.log('Moved:', folder);
  } catch (e) {
    console.log('Error moving', folder, ':', e.message);
  }
});

console.log('Done!');
