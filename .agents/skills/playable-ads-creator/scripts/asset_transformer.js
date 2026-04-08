const fs = require('fs');
const path = require('path');

function getMimeType(filePath) {
    const ext = path.extname(filePath).toLowerCase();
    switch (ext) {
        case '.png': return 'image/png';
        case '.jpg': case '.jpeg': return 'image/jpeg';
        case '.webp': return 'image/webp';
        case '.mp3': return 'audio/mp3';
        case '.wav': return 'audio/wav';
        case '.ogg': return 'audio/ogg';
        default: return 'application/octet-stream';
    }
}

function processAssets(dirPath) {
    if (!fs.existsSync(dirPath)) {
        console.error("Path not found: ", dirPath);
        process.exit(1);
    }

    const files = fs.readdirSync(dirPath);
    let assets = {};
    let totalSize = 0;

    for (const file of files) {
        const fullPath = path.join(dirPath, file);
        const stat = fs.statSync(fullPath);
        if (stat.isDirectory()) continue;

        const mime = getMimeType(file);
        if (mime === 'application/octet-stream') continue;

        console.log(`Processing: ${file}...`);
        const fileData = fs.readFileSync(fullPath);
        const base64Data = fileData.toString('base64');
        const dataUri = `data:${mime};base64,${base64Data}`;
        
        totalSize += Buffer.from(dataUri).length;
        assets[file] = dataUri;
    }

    const outputJson = path.join(dirPath, 'assets_b64.json');
    fs.writeFileSync(outputJson, JSON.stringify(assets, null, 2));

    const totalMb = totalSize / (1024 * 1024);
    console.log("====================================");
    console.log(`DONE. Created: ${outputJson}`);
    console.log(`Estimated Base64 size: ${totalMb.toFixed(2)} MB`);
    if (totalMb > 5) {
        console.log("WARNING: Size > 5MB limit!");
    } else {
        console.log("SUCCESS: Size is within limit.");
    }
}

const targetDir = process.argv[2];
if (!targetDir) {
    console.log("Usage: node asset_transformer.js <path_to_assets>");
    process.exit(1);
}
processAssets(targetDir);
