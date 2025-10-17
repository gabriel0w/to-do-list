import { test, expect } from '@playwright/test';

test.describe('Tasks E2E', () => {
  test('CRUD + toggle + reorder', async ({ page }) => {
    await page.goto('/tasks');

    // Create two tasks
    await page.getByTestId('title-input').fill('Task A');
    await page.getByTestId('add-btn').click();
    await expect(page.getByText('Task A')).toBeVisible();

    await page.getByTestId('title-input').fill('Task B');
    await page.getByTestId('add-btn').click();
    await expect(page.getByText('Task B')).toBeVisible();

    // Toggle first
    const firstItem = page.getByTestId(/item-/).first();
    const firstId = await firstItem.getAttribute('data-testid');
    if (firstId) {
      const id = firstId.split('-')[1];
      await page.getByTestId(`toggle-${id}`).click();
    }

    // Reorder by dragging last up
    const items = page.getByTestId(/item-/);
    const count = await items.count();
    if (count >= 2) {
      const last = items.nth(count - 1);
      const first = items.nth(0);
      await last.dragTo(first);
    }

    // Delete one
    const anyItem = page.getByTestId(/item-/).first();
    const anyId = await anyItem.getAttribute('data-testid');
    if (anyId) {
      const id = anyId.split('-')[1];
      await page.getByTestId(`delete-${id}`).click();
    }

    // List still visible
    await expect(page.getByTestId('list')).toBeVisible();
  });
});

