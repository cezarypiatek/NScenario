import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { ExceptionCtr } from './App';

const exception = `System.InvalidOperationException: Something wrong
   at NScenario.Demo.Tests.should_fail() in C:\\repos\\NScenario\\Tests.cs:line 160
   at a line that does not contain a source location`;

test('formats stack frames as file links by default', () => {
  render(<ExceptionCtr exception={exception} />);

  const methodLink = screen.getByRole('link', { name: 'NScenario.Demo.Tests.should_fail()' });
  expect(methodLink).toHaveAttribute('href', 'C:\\repos\\NScenario\\Tests.cs');
  expect(methodLink).toHaveAttribute('title', 'in C:\\repos\\NScenario\\Tests.cs:line 160');
  expect(screen.getByRole('button', { name: 'Formatted' })).toHaveAttribute('aria-pressed', 'true');
  expect(screen.getByText(/at a line that does not contain a source location/)).toBeInTheDocument();
  expect(screen.queryByText(/in C:\\repos\\NScenario\\Tests\.cs:line 160/)).not.toBeInTheDocument();
});

test('can switch between formatted and raw call stacks', () => {
  render(<ExceptionCtr exception={exception} />);

  fireEvent.click(screen.getByRole('button', { name: 'Raw' }));

  expect(screen.queryByRole('link')).not.toBeInTheDocument();
  expect(screen.getByRole('button', { name: 'Raw' })).toHaveAttribute('aria-pressed', 'true');
  expect(screen.getByText(/NScenario\.Demo\.Tests\.should_fail/)).toBeInTheDocument();
  expect(screen.getByText(/in C:\\repos\\NScenario\\Tests\.cs:line 160/)).toBeInTheDocument();
});
