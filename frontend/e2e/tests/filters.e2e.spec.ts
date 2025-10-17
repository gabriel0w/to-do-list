import { test, expect } from '@playwright/test';

test.describe('Task filters', () => {
  test('status and sorting filters', async ({ page }) => {
    await page.goto('/tasks');

    // Ensure at least one item exists; if empty, create a couple
    const list = page.getByTestId('list');
    const empty = page.getByTestId('empty');
    if (await empty.isVisible().catch(() => false)) {
      await page.getByTestId('title-input').fill('E2E A');
      await page.getByTestId('add-btn').click();
      await page.getByTestId('title-input').fill('E2E B');
      await page.getByTestId('add-btn').click();
    }

    // Mark one as done
    const first = page.getByTestId(/item-/).first();
    const idAttr = await first.getAttribute('data-testid');
    if (idAttr) {
      const id = idAttr.split('-')[1];
      await page.getByTestId(`toggle-${id}`).click();
    }

    // Show only done
    await page.getByTestId('status-select').click();
    await page.getByRole('option', { name: 'Done' }).click();
    await expect(list).toBeVisible();
    const doneCount = await page.getByTestId(/item-/).count();
    expect(doneCount).toBeGreaterThan(0);

    // Show only open
    await page.getByTestId('status-select').click();
    await page.getByRole('option', { name: 'Open' }).click();
    const openCount = await page.getByTestId(/item-/).count();
    expect(openCount).toBeGreaterThan(0);

    // Sort by created desc
    await page.getByTestId('sort-select').click();
    await page.getByRole('option', { name: 'Created' }).click();
    await page.getByTestId('direction-select').click();
    await page.getByRole('option', { name: 'Desc' }).click();
    await expect(list).toBeVisible();
  });
});

