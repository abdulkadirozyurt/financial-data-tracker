import { writeFile } from 'node:fs/promises';

const sourceUrl = 'http://localhost:5250/openapi/v1.json';
const targetPath = new URL('../openapi.json', import.meta.url);

const response = await fetch(sourceUrl);

if (!response.ok) {
  throw new Error(
    `Failed to fetch OpenAPI document from ${sourceUrl}: ${response.status} ${response.statusText}`,
  );
}

const text = await response.text();
await writeFile(targetPath, text, 'utf8');
